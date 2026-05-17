using System;
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
            ViewBag.TotalBooks = db.Books.Count();
            ViewBag.ActiveBorrows = db.BorrowingTransactions.Count(b => !b.IsReturned);
            ViewBag.OverdueBooks = db.BorrowingTransactions.Count(b =>
                !b.IsReturned && b.DueDate < DateTime.Now);

            var recentBorrows = db.BorrowingTransactions
                .OrderByDescending(b => b.BorrowDate)
                .Take(10)
                .ToList();

            return View(recentBorrows);
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