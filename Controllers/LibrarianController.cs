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
            var totalBooks = db.Books.Count();
            var activeBorrows = db.BorrowingTransactions.Count(b => !b.IsReturned);
            var overdueBooks = db.BorrowingTransactions.Count(b =>
                !b.IsReturned && b.DueDate < DateTime.Now);

            ViewBag.TotalBooks = totalBooks;
            ViewBag.ActiveBorrows = activeBorrows;
            ViewBag.OverdueBooks = overdueBooks;

            var recentBorrows = db.BorrowingTransactions
                .OrderByDescending(b => b.BorrowDate)
                .Take(10)
                .ToList();

            return View(recentBorrows);
        }
    }
}