using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class BorrowingTransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =========================
        // LIST ALL BORROWED BOOKS
        // =========================
        public ActionResult Index()
        {
            var data = db.BorrowingTransactions
                .Include("Book")
                .Include("User")
                .OrderByDescending(x => x.BorrowDate)
                .ToList();

            return View(data);
        }

        // =========================
        // BORROW BOOK (MAIN LOGIC)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Borrow(int bookId)
        {
            var book = db.Books.Find(bookId);

            if (book == null)
                return HttpNotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] = "No copies available for this book.";
                return RedirectToAction("Index", "Books");
            }

            var userId = User.Identity.GetUserId();

            var transaction = new BorrowingTransaction
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false,
                RenewalCount = 0
            };

            book.AvailableCopies--;

            db.BorrowingTransactions.Add(transaction);
            db.SaveChanges();

            TempData["Success"] = "Book borrowed successfully!";
            return RedirectToAction("Index", "Books");
        }

        // =========================
        // RETURN BOOK
        // =========================
        public ActionResult Return(int id)
        {
            var transaction = db.BorrowingTransactions.Find(id);

            if (transaction == null)
                return HttpNotFound();

            if (transaction.IsReturned)
            {
                TempData["Error"] = "Book already returned.";
                return RedirectToAction("Index");
            }

            transaction.IsReturned = true;
            transaction.ReturnDate = DateTime.Now;

            var book = db.Books.Find(transaction.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
            }

            db.SaveChanges();

            TempData["Success"] = "Book returned successfully!";
            return RedirectToAction("Index");
        }

        // =========================
        // DETAILS
        // =========================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var data = db.BorrowingTransactions
                .Include("Book")
                .Include("User")
                .FirstOrDefault(x => x.Id == id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        // =========================
        // CLEAN UP
        // =========================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}