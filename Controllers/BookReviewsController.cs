using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize]
    public class BookReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /BookReviews/Index — Librarian moderation
        [Authorize(Roles = "Librarian,Admin")]
        public ActionResult Index()
        {
            var reviews = db.BookReviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            return View(reviews);
        }

        // GET: /BookReviews/Create?bookId=5
        public ActionResult Create(int? bookId)
        {
            if (bookId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var book = db.Books.Find(bookId);
            if (book == null)
                return HttpNotFound();

            var userId = User.Identity.GetUserId();

            // One review per user per book — redirect to edit if they already have one
            var existing = db.BookReviews
                .FirstOrDefault(r => r.BookId == bookId && r.UserId == userId);

            if (existing != null)
            {
                TempData["Info"] = "You've already reviewed this book. You can edit your review below.";
                return RedirectToAction("Edit", new { id = existing.Id });
            }

            ViewBag.Book = book;
            return View(new BookReview { BookId = book.Id });
        }

        // POST: /BookReviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,Rating,ReviewText")] BookReview review)
        {
            // UserId is set from the logged-in user, not the form — remove its validation error
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                review.UserId = User.Identity.GetUserId();
                review.CreatedDate = DateTime.Now;

                // Double-check no duplicate (in case of race condition)
                var existing = db.BookReviews
                    .FirstOrDefault(r => r.BookId == review.BookId && r.UserId == review.UserId);

                if (existing != null)
                {
                    TempData["Info"] = "You've already reviewed this book.";
                    return RedirectToAction("Edit", new { id = existing.Id });
                }

                db.BookReviews.Add(review);
                db.SaveChanges();

                TempData["Success"] = "Thanks for your review!";
                return RedirectToAction("Details", "Books", new { id = review.BookId });
            }

            ViewBag.Book = db.Books.Find(review.BookId);
            return View(review);
        }

        // POST: /BookReviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BookId,Rating,ReviewText")] BookReview review)
        {
            // UserId isn't part of the form — clear the validation error
            ModelState.Remove("UserId");

            var existing = db.BookReviews.Find(review.Id);
            if (existing == null)
                return HttpNotFound();

            // Ownership check
            var userId = User.Identity.GetUserId();
            if (existing.UserId != userId && !User.IsInRole("Librarian") && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                existing.Rating = review.Rating;
                existing.ReviewText = review.ReviewText;
                existing.UpdatedDate = DateTime.Now;
                db.SaveChanges();

                TempData["Success"] = "Review updated.";
                return RedirectToAction("Details", "Books", new { id = existing.BookId });
            }

            ViewBag.Book = db.Books.Find(review.BookId);
            return View(review);
        }

        // POST: /BookReviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BookId,Rating,ReviewText")] BookReview review)
        {
            var existing = db.BookReviews.Find(review.Id);
            if (existing == null)
                return HttpNotFound();

            // Ownership check
            var userId = User.Identity.GetUserId();
            if (existing.UserId != userId && !User.IsInRole("Librarian") && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                existing.Rating = review.Rating;
                existing.ReviewText = review.ReviewText;
                existing.UpdatedDate = DateTime.Now;
                db.SaveChanges();

                TempData["Success"] = "Review updated.";
                return RedirectToAction("Details", "Books", new { id = existing.BookId });
            }

            ViewBag.Book = db.Books.Find(review.BookId);
            return View(review);
        }

        // POST: /BookReviews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var review = db.BookReviews.Find(id);
            if (review == null)
                return HttpNotFound();

            // Owner or librarian can delete
            var userId = User.Identity.GetUserId();
            if (review.UserId != userId && !User.IsInRole("Librarian") && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var bookId = review.BookId;
            db.BookReviews.Remove(review);
            db.SaveChanges();

            TempData["Success"] = "Review deleted.";

            // Librarians come from the moderation page, members come from book details
            if (User.IsInRole("Librarian") && Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.Contains("/BookReviews"))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}