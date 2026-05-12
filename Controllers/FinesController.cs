using LibrarySystem99.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class FinesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Fines
        [Authorize(Roles = "Member,Librarian")]
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

            if (User.IsInRole("Librarian"))
            {
                // librarian sees ALL fines
                var allFines = db.Fines.Include("BorrowingTransaction").ToList();
                return View(allFines);
            }

            // member sees ONLY their own fines
            var myFines = db.Fines
                .Where(f => f.BorrowingTransaction.UserId == currentUserId)
                .ToList();

            return View(myFines);
        }

        // GET: Fines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fine fine = db.Fines.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            return View(fine);
        }

        // GET: Fines/Create
        [Authorize(Roles ="Librarian")]
        public ActionResult Create()
        {
            ViewBag.BorrowingTransactionId = new SelectList(db.BorrowingTransactions, "Id", "UserId");
            return View();
        }

        // POST: Fines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BorrowingTransactionId,Amount,IsPaid,CreatedDate")] Fine fine)
        {
            if (ModelState.IsValid)
            {
                db.Fines.Add(fine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BorrowingTransactionId = new SelectList(db.BorrowingTransactions, "Id", "UserId", fine.BorrowingTransactionId);
            return View(fine);
        }

        // GET: Fines/Edit/5
        [Authorize(Roles ="Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fine fine = db.Fines.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            ViewBag.BorrowingTransactionId = new SelectList(db.BorrowingTransactions, "Id", "UserId", fine.BorrowingTransactionId);
            return View(fine);
        }

        // POST: Fines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BorrowingTransactionId,Amount,IsPaid,CreatedDate")] Fine fine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BorrowingTransactionId = new SelectList(db.BorrowingTransactions, "Id", "UserId", fine.BorrowingTransactionId);
            return View(fine);
        }

        // GET: Fines/Delete/5
        [Authorize(Roles ="Librarian")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fine fine = db.Fines.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            return View(fine);
        }

        // POST: Fines/Delete/5
        [Authorize(Roles ="Librarian")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fine fine = db.Fines.Find(id);
            db.Fines.Remove(fine);
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

        //Pay option for member to pay fine
        [Authorize]
        public ActionResult Pay(int id)
        {
            var fine = db.Fines.Find(id);

            if (fine == null)
            {
                return HttpNotFound();
            }

            fine.IsPaid = true;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
