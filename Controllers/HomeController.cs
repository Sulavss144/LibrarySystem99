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
        public ActionResult Index()
        {
            // Seed roles on first visit (idempotent)
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists("Librarian"))
            {
                roleManager.Create(new IdentityRole("Librarian"));
            }

            if (!roleManager.RoleExists("Member"))
            {
                roleManager.Create(new IdentityRole("Member"));
            }

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

            // Anonymous users see the public landing page
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
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
    }
}