using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var myBooks = db.BorrowingTransactions
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToList();

            return View(myBooks);
        }
    }
}