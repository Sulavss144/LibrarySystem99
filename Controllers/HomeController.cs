using LibrarySystem99.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibrarySystem99.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // Seed roles on first visit (idempotent)
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!roleManager.RoleExists("Librarian"))
            {
                roleManager.Create(new IdentityRole("Librarian"));
            }

            if (!roleManager.RoleExists("Member"))
            {
                roleManager.Create(new IdentityRole("Member"));
            }

            // Seed default borrowing policy if none exists
            PolicyHelper.EnsureDefaultPolicyExists(db);

            // Redirect authenticated users to their role-specific dashboard
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Librarian"))
                {
                    return RedirectToAction("Index", "Librarian");
                }

                if (User.IsInRole("Member"))
                {
                    return RedirectToAction("Index", "Member");
                }
            }

            // Load shared dashboard widgets (New Arrivals, Most Borrowed, Available)
            DashboardHelper.LoadCommonData(this, db);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            using (var feedbackDb = new ApplicationDbContext())
            {
                var recentFeedback = feedbackDb.WebsiteFeedbacks
                    .Where(f => f.IsApproved)
                    .OrderByDescending(f => f.CreatedDate)
                    .Take(10)
                    .ToList();

                ViewBag.RecentFeedback = recentFeedback;
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // GET: /Home/LibrarianDashboard
        // Kept for backwards compatibility. Redirects to /Librarian/Index.
        [Authorize(Roles = "Librarian")]
        public ActionResult LibrarianDashboard()
        {
            return RedirectToAction("Index", "Librarian");
        }

        // GET: /Home/MemberDashboard
        // Kept for backwards compatibility. Redirects to /Member/Index.
        [Authorize(Roles = "Member")]
        public ActionResult MemberDashboard()
        {
            return RedirectToAction("Index", "Member");
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