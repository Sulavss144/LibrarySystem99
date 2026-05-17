using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class MembersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: /Members/Index
        public ActionResult Index()
        {
            // Get all users who are in the "Member" role
            var memberRole = db.Roles.FirstOrDefault(r => r.Name == "Member");
            if (memberRole == null)
            {
                return View(new System.Collections.Generic.List<ApplicationUser>());
            }

            var members = db.Users
                .Where(u => u.Roles.Any(r => r.RoleId == memberRole.Id))
                .OrderBy(u => u.Email)
                .ToList();

            return View(members);
        }

        // GET: /Members/Details/{id}
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var member = db.Users.Find(id);
            if (member == null)
                return HttpNotFound();

            // Borrowing history for this member
            var borrowings = db.BorrowingTransactions
                .Include(b => b.Book)
                .Where(b => b.UserId == id)
                .OrderByDescending(b => b.BorrowDate)
                .ToList();

            // Fines for this member
            var fines = db.Fines
                .Include(f => f.BorrowingTransaction)
                .Where(f => f.BorrowingTransaction.UserId == id)
                .ToList();

            ViewBag.Borrowings = borrowings;
            ViewBag.Fines = fines;
            ViewBag.TotalBorrowed = borrowings.Count;
            ViewBag.CurrentlyBorrowed = borrowings.Count(b => !b.IsReturned);
            ViewBag.OverdueCount = borrowings.Count(b => !b.IsReturned && b.DueDate < DateTime.Now);
            ViewBag.UnpaidFines = fines.Where(f => !f.IsPaid).Sum(f => (decimal?)f.Amount) ?? 0m;

            return View(member);
        }

        // GET: /Members/Edit/{id}
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var member = db.Users.Find(id);
            if (member == null)
                return HttpNotFound();

            return View(member);
        }

        // POST: /Members/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, string Email, string PhoneNumber)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var member = await UserManager.FindByIdAsync(id);
            if (member == null)
                return HttpNotFound();

            member.Email = Email;
            member.UserName = Email; // keep UserName in sync with Email (Identity convention)
            member.PhoneNumber = PhoneNumber;

            var result = await UserManager.UpdateAsync(member);

            if (result.Succeeded)
            {
                TempData["Success"] = "Member updated successfully.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(member);
        }

        // GET: /Members/Delete/{id}
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var member = db.Users.Find(id);
            if (member == null)
                return HttpNotFound();

            // Safety check: don't allow deletion if member has active borrowings
            var hasActiveBorrowings = db.BorrowingTransactions
                .Any(b => b.UserId == id && !b.IsReturned);

            ViewBag.HasActiveBorrowings = hasActiveBorrowings;

            return View(member);
        }

        // POST: /Members/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var member = await UserManager.FindByIdAsync(id);
            if (member == null)
                return HttpNotFound();

            // Safety check: prevent deletion if member has active borrowings
            var hasActiveBorrowings = db.BorrowingTransactions
                .Any(b => b.UserId == id && !b.IsReturned);

            if (hasActiveBorrowings)
            {
                TempData["Error"] = "Cannot delete this member — they have active borrowings. Please ensure all books are returned first.";
                return RedirectToAction("Delete", new { id = id });
            }

            var result = await UserManager.DeleteAsync(member);

            if (result.Succeeded)
            {
                TempData["Success"] = "Member deleted successfully.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(member);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}