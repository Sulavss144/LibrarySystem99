using System;
using System.Collections.Generic;

namespace LibrarySystem99.Models
{
    public class ReportsViewModel
    {
        // 1. Overdue books
        public List<BorrowingTransaction> OverdueBooks { get; set; }

        // 2. Most popular books (by borrow count)
        public List<PopularBookRow> PopularBooks { get; set; }

        // 3. Member activity
        public List<MemberActivityRow> TopBorrowers { get; set; }
        public List<ApplicationUser> InactiveMembers { get; set; }

        // 4. Fines summary
        public decimal TotalCollected { get; set; }
        public decimal TotalUnpaid { get; set; }
        public List<MemberFineRow> FinesByMember { get; set; }

        // 5. Borrowing trends
        public List<BorrowTrendRow> BorrowsPerDay { get; set; }
        public List<BorrowTrendRow> BorrowsPerMonth { get; set; }

        // 6. Inventory status
        public List<Book> LowStockBooks { get; set; }
        public List<Book> FullyBorrowedBooks { get; set; }

        // Top-level KPIs
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int TotalBorrows { get; set; }
    }

    public class PopularBookRow
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int BorrowCount { get; set; }
    }

    public class MemberActivityRow
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public int BorrowCount { get; set; }
    }

    public class MemberFineRow
    {
        public string Email { get; set; }
        public decimal TotalFines { get; set; }
        public decimal UnpaidFines { get; set; }
    }

    public class BorrowTrendRow
    {
        public string Period { get; set; }
        public int Count { get; set; }
    }
}