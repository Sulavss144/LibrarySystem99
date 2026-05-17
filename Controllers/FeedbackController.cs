using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    public class FeedbackController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Feedback/Index — Librarian moderation page
        [Authorize(Roles = "Librarian,Admin")]
        public ActionResult Index()
        {
            var feedback = db.WebsiteFeedbacks
                .OrderByDescending(f => f.CreatedDate)
                .ToList();

            ViewBag.Total = feedback.Count;
            ViewBag.Approved = feedback.Count(f => f.IsApproved);
            ViewBag.Hidden = feedback.Count(f => !f.IsApproved);

            return View(feedback);
        }

        // POST: /Feedback/Create — Anyone (no auth required)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Email,Message")] WebsiteFeedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.CreatedDate = DateTime.Now;
                feedback.IsApproved = true;
                db.WebsiteFeedbacks.Add(feedback);
                db.SaveChanges();

                TempData["Success"] = "Thank you for your feedback! It has been posted.";
                return RedirectToAction("About", "Home");
            }

            TempData["Error"] = "Please correct the form errors and try again.";
            return RedirectToAction("About", "Home");
        }

        // POST: /Feedback/ToggleApproval/{id} — Librarian shows/hides
        [HttpPost]
        [Authorize(Roles = "Librarian,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleApproval(int id)
        {
            var item = db.WebsiteFeedbacks.Find(id);
            if (item == null)
                return HttpNotFound();

            item.IsApproved = !item.IsApproved;
            db.SaveChanges();

            TempData["Success"] = item.IsApproved
                ? "Feedback is now visible on the About page."
                : "Feedback has been hidden from the About page.";
            return RedirectToAction("Index");
        }
        // POST: /Feedback/Reply/{id} — Librarian writes/updates a reply
        [HttpPost]
        [Authorize(Roles = "Librarian,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int id, string replyText)
        {
            var item = db.WebsiteFeedbacks.Find(id);
            if (item == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(replyText))
            {
                // Empty reply = clear the existing reply
                item.LibrarianReply = null;
                item.RepliedDate = null;
                TempData["Success"] = "Reply cleared.";
            }
            else
            {
                item.LibrarianReply = replyText.Trim();
                item.RepliedDate = DateTime.Now;
                TempData["Success"] = "Reply posted successfully.";
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: /Feedback/Delete/{id} — Librarian permanent delete
        [HttpPost]
        [Authorize(Roles = "Librarian,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var item = db.WebsiteFeedbacks.Find(id);
            if (item == null)
                return HttpNotFound();

            db.WebsiteFeedbacks.Remove(item);
            db.SaveChanges();

            TempData["Success"] = "Feedback deleted permanently.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}