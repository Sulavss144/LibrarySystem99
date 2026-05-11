using System.ComponentModel.DataAnnotations;

namespace LibrarySystem99.Models
{
    public class BorrowingPolicy
    {
        public int Id { get; set; }

        [Required]
        public int MaxBooksPerUser { get; set; }

        [Required]
        public int BorrowDays { get; set; }

        [Required]
        public int MaxRenewals { get; set; }

        [Required]
        public decimal FinePerDay { get; set; }
    }
}