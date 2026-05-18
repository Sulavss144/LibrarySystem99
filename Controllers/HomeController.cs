using LibrarySystem99.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace LibrarySystem99.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // checking whether the role exists, if not create a new role
            // here i have added this line to test the git commit and push functionality
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
        [Authorize(Roles = "Librarian")]
        public ActionResult LibrarianDashboard()
        {
            return View();
        }

        // GET: /Home/MemberDashboard
        [Authorize(Roles = "Member")]
        [Authorize(Roles = "Member")]
        public ActionResult MemberDashboard()
        {
            var context = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();

            var borrowings = context.BorrowingTransactions
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToList();

            return View(borrowings);
        }
    }
}