using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize(Roles = "Librarian,Admin,Member")]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =========================
        // 🔥 SHARED DASHBOARD DATA
        // =========================
        public ReportsViewModel GetDashboardData()
        {
            var model = new ReportsViewModel();

            // ===== HOME / DASHBOARD SHARED BOOK DATA =====
            model.NewArrivals = db.Books
                .OrderByDescending(b => b.Id)
                .Take(8)
                .ToList();

            model.MostBorrowed = db.BorrowingTransactions
                .GroupBy(b => b.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(8)
                .ToList()
                .Select(x => db.Books.FirstOrDefault(b => b.Id == x.BookId))
                .Where(b => b != null)
                .ToList();

            model.AvailableBooks = db.Books
                .Where(b => b.AvailableCopies > 0)
                .OrderByDescending(b => b.Id)
                .Take(8)
                .ToList();

            return model;
        }

        // =========================
        // REPORT PAGE (FULL REPORTS)
        // =========================
        public ActionResult Index()
        {
            var model = GetDashboardData();

            model.TotalBooks = db.Books.Count();
            model.TotalBorrows = db.BorrowingTransactions.Count();

            model.TotalMembers = db.Users.Count();

            model.OverdueBooks = db.BorrowingTransactions
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => !b.IsReturned && b.DueDate < DateTime.Now)
                .ToList();

            model.PopularBooks = db.BorrowingTransactions
                .GroupBy(b => b.BookId)
                .Select(g => new PopularBookRow
                {
                    Title = db.Books.Where(b => b.Id == g.Key).Select(b => b.Title).FirstOrDefault(),
                    Author = db.Books.Where(b => b.Id == g.Key).Select(b => b.Author).FirstOrDefault(),
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(10)
                .ToList();

            return View(model);
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