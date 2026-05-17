using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LibrarySystem99.Models;

namespace LibrarySystem99.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var now = DateTime.Now;
            var thirtyDaysAgo = now.AddDays(-30);

            var model = new ReportsViewModel();

            // ===== TOP-LEVEL KPIs =====
            model.TotalBooks = db.Books.Count();
            model.TotalBorrows = db.BorrowingTransactions.Count();

            // Count members (users in "Member" role)
            var memberRole = db.Roles.FirstOrDefault(r => r.Name == "Member");
            model.TotalMembers = memberRole != null
                ? db.Users.Count(u => u.Roles.Any(r => r.RoleId == memberRole.Id))
                : 0;

            // ===== 1. OVERDUE BOOKS =====
            model.OverdueBooks = db.BorrowingTransactions
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => !b.IsReturned && b.DueDate < now)
                .OrderBy(b => b.DueDate)
                .ToList();

            // ===== 2. MOST POPULAR BOOKS =====
            model.PopularBooks = db.BorrowingTransactions
                .GroupBy(b => b.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList()
                .Select(x =>
                {
                    var book = db.Books.Find(x.BookId);
                    return new PopularBookRow
                    {
                        Title = book?.Title ?? "—",
                        Author = book?.Author ?? "—",
                        BorrowCount = x.Count
                    };
                })
                .ToList();

            // ===== 3. MEMBER ACTIVITY =====
            // Top borrowers
            var memberRoleId = memberRole?.Id;
            var memberIds = memberRoleId != null
                ? db.Users.Where(u => u.Roles.Any(r => r.RoleId == memberRoleId)).Select(u => u.Id).ToList()
                : new System.Collections.Generic.List<string>();

            model.TopBorrowers = db.BorrowingTransactions
                .Where(b => memberIds.Contains(b.UserId))
                .GroupBy(b => b.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList()
                .Select(x =>
                {
                    var user = db.Users.Find(x.UserId);
                    return new MemberActivityRow
                    {
                        Email = user?.Email ?? "—",
                        FullName = user?.FullName ?? "—",
                        BorrowCount = x.Count
                    };
                })
                .ToList();

            // Inactive members (no borrows in last 30 days)
            var activeUserIds = db.BorrowingTransactions
                .Where(b => b.BorrowDate >= thirtyDaysAgo)
                .Select(b => b.UserId)
                .Distinct()
                .ToList();

            model.InactiveMembers = db.Users
                .Where(u => memberIds.Contains(u.Id) && !activeUserIds.Contains(u.Id))
                .OrderBy(u => u.Email)
                .ToList();

            // ===== 4. FINES SUMMARY =====
            model.TotalCollected = db.Fines.Where(f => f.IsPaid).Sum(f => (decimal?)f.Amount) ?? 0m;
            model.TotalUnpaid = db.Fines.Where(f => !f.IsPaid).Sum(f => (decimal?)f.Amount) ?? 0m;

            model.FinesByMember = db.Fines
                .Include(f => f.BorrowingTransaction)
                .ToList()
                .GroupBy(f => f.BorrowingTransaction.UserId)
                .Select(g =>
                {
                    var user = db.Users.Find(g.Key);
                    return new MemberFineRow
                    {
                        Email = user?.Email ?? "—",
                        TotalFines = g.Sum(f => f.Amount),
                        UnpaidFines = g.Where(f => !f.IsPaid).Sum(f => f.Amount)
                    };
                })
                .OrderByDescending(r => r.TotalFines)
                .ToList();

            // ===== 5. BORROWING TRENDS =====
            // Borrows per day (last 14 days)
            var fourteenDaysAgo = now.AddDays(-13).Date;
            var dailyBorrows = db.BorrowingTransactions
                .Where(b => b.BorrowDate >= fourteenDaysAgo)
                .ToList()
                .GroupBy(b => b.BorrowDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            model.BorrowsPerDay = new System.Collections.Generic.List<BorrowTrendRow>();
            for (int i = 0; i < 14; i++)
            {
                var day = fourteenDaysAgo.AddDays(i);
                model.BorrowsPerDay.Add(new BorrowTrendRow
                {
                    Period = day.ToString("dd MMM"),
                    Count = dailyBorrows.ContainsKey(day) ? dailyBorrows[day] : 0
                });
            }

            // Borrows per month (last 6 months)
            var sixMonthsAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
            var monthlyBorrows = db.BorrowingTransactions
                .Where(b => b.BorrowDate >= sixMonthsAgo)
                .ToList()
                .GroupBy(b => new { b.BorrowDate.Year, b.BorrowDate.Month })
                .ToDictionary(g => new DateTime(g.Key.Year, g.Key.Month, 1), g => g.Count());

            model.BorrowsPerMonth = new System.Collections.Generic.List<BorrowTrendRow>();
            for (int i = 0; i < 6; i++)
            {
                var month = sixMonthsAgo.AddMonths(i);
                model.BorrowsPerMonth.Add(new BorrowTrendRow
                {
                    Period = month.ToString("MMM yyyy"),
                    Count = monthlyBorrows.ContainsKey(month) ? monthlyBorrows[month] : 0
                });
            }

            // ===== 6. INVENTORY STATUS =====
            model.LowStockBooks = db.Books
                .Where(b => b.AvailableCopies > 0 && b.AvailableCopies <= 2)
                .OrderBy(b => b.AvailableCopies)
                .ToList();

            model.FullyBorrowedBooks = db.Books
                .Where(b => b.AvailableCopies == 0 && b.TotalCopies > 0)
                .OrderBy(b => b.Title)
                .ToList();

            return View(model);
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