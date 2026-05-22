using System;
using System.Collections.Generic;

namespace LibrarySystem99.Models
{
    public class ReportsViewModel
    {
        // ===== Shared Dashboard Data =====
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int TotalBorrows { get; set; }

        // ===== Homepage + Dashboards =====
        public List<Book> NewArrivals { get; set; } = new List<Book>();
        public List<Book> AvailableBooks { get; set; } = new List<Book>();
        public List<Book> MostBorrowed { get; set; } = new List<Book>();

        // ===== Reports Section =====
        public List<BorrowingTransaction> OverdueBooks { get; set; } = new List<BorrowingTransaction>();
        public List<PopularBookRow> PopularBooks { get; set; } = new List<PopularBookRow>();
        public List<MemberActivityRow> TopBorrowers { get; set; } = new List<MemberActivityRow>();
        public List<ApplicationUser> InactiveMembers { get; set; } = new List<ApplicationUser>();

        public decimal TotalCollected { get; set; }
        public decimal TotalUnpaid { get; set; }

        public List<MemberFineRow> FinesByMember { get; set; } = new List<MemberFineRow>();

        public List<BorrowTrendRow> BorrowsPerDay { get; set; } = new List<BorrowTrendRow>();
        public List<BorrowTrendRow> BorrowsPerMonth { get; set; } = new List<BorrowTrendRow>();

        public List<Book> LowStockBooks { get; set; } = new List<Book>();
        public List<Book> FullyBorrowedBooks { get; set; } = new List<Book>();
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