using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LibrarySystem99.Models
{
    public static class DashboardHelper
    {
        public static void LoadCommonData(Controller controller, ApplicationDbContext db)
        {
            var books = db.Books.ToList();

            // New Arrivals — latest 6 books by Id
            controller.ViewBag.NewArrivals = books
                .OrderByDescending(b => b.Id)
                .Take(6)
                .ToList();

            // Available Books — books with copies available
            controller.ViewBag.AvailableBooks = books
                .Where(b => b.AvailableCopies > 0)
                .OrderByDescending(b => b.AvailableCopies)
                .Take(6)
                .ToList();

            // Most Borrowed — top 6 books by borrow count
            var topIds = db.BorrowingTransactions
                .GroupBy(b => b.BookId)
                .Select(g => new { BookId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(6)
                .ToList();

            controller.ViewBag.MostBorrowed = topIds
                .Select(x => books.FirstOrDefault(b => b.Id == x.BookId))
                .Where(b => b != null)
                .ToList();
        }
    }
}