using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Reservations — Member sees own, Librarian sees all
        public ActionResult Index()
        {
            if (User.IsInRole("Librarian") || User.IsInRole("Admin"))
            {
                var all = db.Reservations
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.ReservationDate)
                    .ToList();
                return View(all);
            }

            var userId = User.Identity.GetUserId();
            var mine = db.Reservations
                .Include(r => r.Book)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservationDate)
                .ToList();
            return View(mine);
        }

        // POST: /Reservations/Create
        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int bookId)
        {
            var book = db.Books.Find(bookId);
            if (book == null)
                return HttpNotFound();

            // Reservations only allowed when no copies available
            if (book.AvailableCopies > 0)
            {
                TempData["Error"] = "This book has copies available. You can borrow it directly instead of reserving.";
                return RedirectToAction("Index", "Books");
            }

            var userId = User.Identity.GetUserId();

            // Check if user already has an active reservation for this book
            var existing = db.Reservations.FirstOrDefault(r =>
                r.BookId == bookId &&
                r.UserId == userId &&
                (r.Status == ReservationStatus.Waiting || r.Status == ReservationStatus.Ready));

            if (existing != null)
            {
                TempData["Info"] = "You already have an active reservation for this book.";
                return RedirectToAction("Index");
            }

            // Check if user is currently borrowing the same book
            var alreadyBorrowing = db.BorrowingTransactions.Any(b =>
                b.BookId == bookId && b.UserId == userId && !b.IsReturned);

            if (alreadyBorrowing)
            {
                TempData["Error"] = "You're already borrowing this book.";
                return RedirectToAction("Index", "Books");
            }

            var reservation = new Reservation
            {
                BookId = bookId,
                UserId = userId,
                ReservationDate = DateTime.Now,
                Status = ReservationStatus.Waiting
            };

            db.Reservations.Add(reservation);
            db.SaveChanges();

            // Tell the member their position in the queue
            var position = db.Reservations
                .Count(r => r.BookId == bookId &&
                           r.Status == ReservationStatus.Waiting &&
                           r.ReservationDate <= reservation.ReservationDate);

            TempData["Success"] = $"Reservation placed for \"{book.Title}\". You are #{position} in the queue.";
            return RedirectToAction("Index");
        }

        // POST: /Reservations/Cancel/5 — Member cancels own, librarian can cancel any
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var reservation = db.Reservations.Find(id);
            if (reservation == null)
                return HttpNotFound();

            var userId = User.Identity.GetUserId();
            bool isLibrarian = User.IsInRole("Librarian") || User.IsInRole("Admin");

            if (reservation.UserId != userId && !isLibrarian)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (reservation.Status == ReservationStatus.Fulfilled)
            {
                TempData["Error"] = "Cannot cancel a fulfilled reservation.";
                return RedirectToAction("Index");
            }

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelledDate = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = "Reservation cancelled.";
            return RedirectToAction("Index");
        }

        // POST: /Reservations/Fulfill/5 — When member actually picks up & borrows
        [HttpPost]
        [Authorize(Roles = "Librarian,Admin,Member")]
        [ValidateAntiForgeryToken]
        public ActionResult Fulfill(int id)
        {
            var reservation = db.Reservations
                .Include(r => r.Book)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
                return HttpNotFound();

            var userId = User.Identity.GetUserId();
            bool isLibrarian = User.IsInRole("Librarian") || User.IsInRole("Admin");

            if (reservation.UserId != userId && !isLibrarian)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (reservation.Status != ReservationStatus.Ready)
            {
                TempData["Error"] = "This reservation isn't ready for pickup yet.";
                return RedirectToAction("Index");
            }

            if (reservation.Book.AvailableCopies <= 0)
            {
                TempData["Error"] = "No copies available right now. Please try again later.";
                return RedirectToAction("Index");
            }

            // Apply policy enforcement (same as Borrow)
            var policy = PolicyHelper.GetActivePolicy(db);
            var activeBorrows = db.BorrowingTransactions
                .Count(b => b.UserId == reservation.UserId && !b.IsReturned);

            if (activeBorrows >= policy.MaxBooksPerUser)
            {
                TempData["Error"] = $"Borrowing limit reached ({policy.MaxBooksPerUser} books).";
                return RedirectToAction("Index");
            }

            // Create the borrow
            var borrow = new BorrowingTransaction
            {
                BookId = reservation.BookId,
                UserId = reservation.UserId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(policy.BorrowDays),
                IsReturned = false,
                RenewalCount = 0
            };
            db.BorrowingTransactions.Add(borrow);

            reservation.Book.AvailableCopies--;
            reservation.Status = ReservationStatus.Fulfilled;
            reservation.FulfilledDate = DateTime.Now;

            db.SaveChanges();

            TempData["Success"] = $"Reservation fulfilled. Due date: {borrow.DueDate.ToShortDateString()}.";
            return RedirectToAction("MyBooks", "Books");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}