using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class LibrarianController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // Load shared dashboard widgets (New Arrivals, Most Borrowed, Available)
            DashboardHelper.LoadCommonData(this, db);

            // Librarian-specific KPIs
            ViewBag.TotalBooks = db.Books.Count();
            ViewBag.ActiveBorrows = db.BorrowingTransactions.Count(b => !b.IsReturned);
            ViewBag.OverdueBooks = db.BorrowingTransactions.Count(b =>
                !b.IsReturned && b.DueDate < DateTime.Now);

            // Recent borrow activity
            var recentBorrows = db.BorrowingTransactions
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderByDescending(b => b.BorrowDate)
                .Take(10)
                .ToList();

            return View(recentBorrows);
        }

        // GET: /Librarian/ResetData — Confirmation page
        public ActionResult ResetData()
        {
            ViewBag.ActiveBorrows = db.BorrowingTransactions.Count(b => !b.IsReturned);
            ViewBag.TotalFines = db.Fines.Count();
            return View();
        }

        // POST: /Librarian/ResetData
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ResetData")]
        public ActionResult ResetDataConfirmed(string confirmAction)
        {
            int returnedCount = 0;
            int finesCleared = 0;

            if (confirmAction == "ReturnAll" || confirmAction == "ResetAll")
            {
                var activeBorrows = db.BorrowingTransactions
                    .Include(b => b.Book)
                    .Where(b => !b.IsReturned)
                    .ToList();

                foreach (var b in activeBorrows)
                {
                    b.IsReturned = true;
                    b.ReturnDate = DateTime.Now;
                    if (b.Book != null)
                    {
                        b.Book.AvailableCopies++;
                        if (b.Book.AvailableCopies > b.Book.TotalCopies)
                        {
                            b.Book.AvailableCopies = b.Book.TotalCopies;
                        }
                    }
                    returnedCount++;
                }
            }

            if (confirmAction == "ResetAll")
            {
                var allFines = db.Fines.ToList();
                finesCleared = allFines.Count;
                db.Fines.RemoveRange(allFines);
            }

            db.SaveChanges();

            TempData["Success"] = $"Cleanup complete: {returnedCount} borrow(s) returned" +
                (finesCleared > 0 ? $", {finesCleared} fine(s) cleared." : ".");

            return RedirectToAction("Index");
        }

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