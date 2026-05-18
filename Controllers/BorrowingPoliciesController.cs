using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class BorrowingPoliciesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BorrowingPolicies — shows the current active policy
        public ActionResult Index()
        {
            // Ensure a policy exists
            PolicyHelper.EnsureDefaultPolicyExists(db);
            var policy = db.BorrowingPolicies.FirstOrDefault();
            return View(policy);
        }

        // GET: BorrowingPolicies/Edit
        public ActionResult Edit()
        {
            PolicyHelper.EnsureDefaultPolicyExists(db);
            var policy = db.BorrowingPolicies.FirstOrDefault();
            if (policy == null)
                return HttpNotFound();

            return View(policy);
        }

        // POST: BorrowingPolicies/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MaxBooksPerUser,BorrowDays,MaxRenewals,FinePerDay")] BorrowingPolicy policy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(policy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Policy updated successfully.";
                return RedirectToAction("Index");
            }
            return View(policy);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}