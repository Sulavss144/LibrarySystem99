using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index(string searchQuery)
        {
            var books = db.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.Trim();
                books = books.Where(b =>
                    b.Title.Contains(q) ||
                    b.Author.Contains(q));
            }

            ViewBag.SearchQuery = searchQuery;
            return View(books.ToList());
        }

        // GET: Details

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            // Load reviews for this book
            var reviews = db.BookReviews
                .Include(r => r.User)
                .Where(r => r.BookId == id)
                .ToList();

            ViewBag.Reviews = reviews;

            // Check if current user has already reviewed
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                ViewBag.UserHasReviewed = reviews.Any(r => r.UserId == userId);
            }
            else
            {
                ViewBag.UserHasReviewed = false;
            }

            return View(book);
        }

        // GET: Create
        [Authorize(Roles = "Librarian")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [Authorize(Roles = "Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include =
            "Id,Title,Author,ISBN,Category,TotalCopies,AvailableCopies,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.AvailableCopies = book.TotalCopies;
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Edit
        [Authorize(Roles = "Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // POST: Edit
        [Authorize(Roles = "Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Delete
        [Authorize(Roles = "Librarian")]
        public ActionResult Delete(int? id)
        {
            var book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // POST: Delete
        [Authorize(Roles = "Librarian")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // =========================================
        // 🔥 USER BORROWED BOOKS
        // =========================================

        [Authorize(Roles = "Member")]
        public ActionResult MyBooks()
        {
            var userId = User.Identity.GetUserId();

            var books = db.BorrowingTransactions
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && !b.IsReturned)
                .ToList();

            return View(books);
        }

        // =========================================
        // BORROW BOOK SYSTEM
        // =========================================

        [Authorize(Roles = "Member")]
        public ActionResult Borrow(int id)
        {
            var book = db.Books.Find(id);

            if (book == null)
                return HttpNotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] = "No copies available!";
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();

            // ===== POLICY ENFORCEMENT =====
            var policy = PolicyHelper.GetActivePolicy(db);

            // Check max books per user
            var activeBorrows = db.BorrowingTransactions
                .Count(b => b.UserId == userId && !b.IsReturned);

            if (activeBorrows >= policy.MaxBooksPerUser)
            {
                TempData["Error"] = $"You've reached the borrowing limit of {policy.MaxBooksPerUser} books. Please return a book before borrowing another.";
                return RedirectToAction("Index");
            }

            // Check for unpaid fines (optional but realistic)
            var unpaidFines = db.Fines
                .Where(f => !f.IsPaid && f.BorrowingTransaction.UserId == userId)
                .Sum(f => (decimal?)f.Amount) ?? 0m;

            if (unpaidFines > 0)
            {
                TempData["Error"] = $"You have unpaid fines totaling ${unpaidFines}. Please pay your fines before borrowing.";
                return RedirectToAction("Index");
            }

            // ===== CREATE BORROW USING POLICY =====
            var borrow = new BorrowingTransaction
            {
                BookId = book.Id,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(policy.BorrowDays),
                IsReturned = false,
                RenewalCount = 0
            };

            book.AvailableCopies--;

            db.BorrowingTransactions.Add(borrow);
            db.SaveChanges();

            TempData["Success"] = $"Book borrowed successfully. Due date: {borrow.DueDate.ToShortDateString()}.";
            return RedirectToAction("MyBooks");
        }

        // =========================================
        // RETURN BOOK SYSTEM (with auto-fine)
        // =========================================

        [Authorize(Roles = "Member")]
        public ActionResult Return(int id)
        {
            var borrow = db.BorrowingTransactions
                .Include(b => b.Book)
                .FirstOrDefault(b => b.Id == id);

            if (borrow == null)
                return HttpNotFound();

            if (!borrow.IsReturned)
            {
                borrow.IsReturned = true;
                borrow.ReturnDate = DateTime.Now;
                borrow.Book.AvailableCopies++;

                // ===== AUTO-FINE IF OVERDUE =====
                if (borrow.ReturnDate.Value > borrow.DueDate)
                {
                    var daysLate = (borrow.ReturnDate.Value - borrow.DueDate).Days;
                    if ((borrow.ReturnDate.Value - borrow.DueDate).TotalHours > 0 && daysLate == 0)
                    {
                        daysLate = 1;
                    }

                    var policy = PolicyHelper.GetActivePolicy(db);
                    var fineAmount = daysLate * policy.FinePerDay;

                    var fine = new Fine
                    {
                        BorrowingTransactionId = borrow.Id,
                        Amount = fineAmount,
                        IsPaid = false,
                        CreatedDate = DateTime.Now
                    };
                    db.Fines.Add(fine);

                    TempData["Error"] = $"Book returned {daysLate} day(s) late. A fine of ${fineAmount} has been added to your account.";
                }
                else
                {
                    TempData["Success"] = "Book returned on time. Thank you!";
                }

                // ===== AUTO-PROMOTE NEXT RESERVATION =====
                var nextReservation = db.Reservations
                    .Where(r => r.BookId == borrow.BookId && r.Status == ReservationStatus.Waiting)
                    .OrderBy(r => r.ReservationDate)
                    .FirstOrDefault();

                if (nextReservation != null)
                {
                    nextReservation.Status = ReservationStatus.Ready;
                    nextReservation.ReadyDate = DateTime.Now;
                    // Don't decrement AvailableCopies — it's still technically available until fulfilled
                }

                db.SaveChanges();
            }

            return RedirectToAction("MyBooks");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}