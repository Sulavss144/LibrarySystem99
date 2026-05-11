using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index()
        {
            return View(db.Books.ToList());
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include =
            "Id,Title,Author,ISBN,Category,TotalCopies,AvailableCopies,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.AvailableCopies = book.TotalCopies;
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {
            var book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // =========================================
        // 🔥 BORROW BOOK SYSTEM (NEW)
        // =========================================

        [Authorize]
        public ActionResult Borrow(int id)
        {
            var book = db.Books.Find(id);

            if (book == null)
                return HttpNotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] = "No copies available!";
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();

            var borrow = new BorrowingTransaction
            {
                BookId = book.Id,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false,
                RenewalCount = 0
            };

            book.AvailableCopies--;

            db.BorrowingTransactions.Add(borrow);
            db.SaveChanges();

            return RedirectToAction("MyBooks");
        }

        // =========================================
        // 🔥 USER BORROWED BOOKS
        // =========================================

        [Authorize]
        public ActionResult MyBooks()
        {
            var userId = User.Identity.GetUserId();

            var books = db.BorrowingTransactions
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && !b.IsReturned)
                .ToList();

            return View(books);
        }

        // =========================================
        // 🔥 RETURN BOOK SYSTEM
        // =========================================

        [Authorize]
        public ActionResult Return(int id)
        {
            var borrow = db.BorrowingTransactions
                .Include(b => b.Book)
                .FirstOrDefault(b => b.Id == id);

            if (borrow == null)
                return HttpNotFound();

            if (!borrow.IsReturned)
            {
                borrow.IsReturned = true;
                borrow.ReturnDate = DateTime.Now;

                borrow.Book.AvailableCopies++;
            }

            db.SaveChanges();

            return RedirectToAction("MyBooks");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}