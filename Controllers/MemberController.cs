using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize(Roles = "Member,Librarian,Admin")]
    public class MemberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var myBooks = db.BorrowingTransactions
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToList();

            // KPIs for the dashboard cards
            ViewBag.TotalBorrowed = myBooks.Count;
            ViewBag.CurrentlyBorrowed = myBooks.Count(b => !b.IsReturned);
            ViewBag.OverdueCount = myBooks.Count(b =>
                !b.IsReturned && b.DueDate < DateTime.Now);

            return View(myBooks);
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