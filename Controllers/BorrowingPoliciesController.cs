using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class BorrowingPoliciesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BorrowingPolicies
        public ActionResult Index()
        {
            return View(db.BorrowingPolicies.ToList());
        }

        // GET: BorrowingPolicies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BorrowingPolicy borrowingPolicy = db.BorrowingPolicies.Find(id);
            if (borrowingPolicy == null)
            {
                return HttpNotFound();
            }
            return View(borrowingPolicy);
        }

        // GET: BorrowingPolicies/Create
        [Authorize(Roles ="Librarian")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BorrowingPolicies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MaxBooksPerUser,BorrowDays,MaxRenewals,FinePerDay")] BorrowingPolicy borrowingPolicy)
        {
            if (ModelState.IsValid)
            {
                db.BorrowingPolicies.Add(borrowingPolicy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(borrowingPolicy);
        }

        // GET: BorrowingPolicies/Edit/5
        [Authorize(Roles = "Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BorrowingPolicy borrowingPolicy = db.BorrowingPolicies.Find(id);
            if (borrowingPolicy == null)
            {
                return HttpNotFound();
            }
            return View(borrowingPolicy);
        }

        // POST: BorrowingPolicies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MaxBooksPerUser,BorrowDays,MaxRenewals,FinePerDay")] BorrowingPolicy borrowingPolicy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrowingPolicy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(borrowingPolicy);
        }

        // GET: BorrowingPolicies/Delete/5
        [Authorize(Roles = "Librarian")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BorrowingPolicy borrowingPolicy = db.BorrowingPolicies.Find(id);
            if (borrowingPolicy == null)
            {
                return HttpNotFound();
            }
            return View(borrowingPolicy);
        }

        // POST: BorrowingPolicies/Delete/5
        [Authorize(Roles = "Librarian")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BorrowingPolicy borrowingPolicy = db.BorrowingPolicies.Find(id);
            db.BorrowingPolicies.Remove(borrowingPolicy);
            db.SaveChanges();
            return RedirectToAction("Index");
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
