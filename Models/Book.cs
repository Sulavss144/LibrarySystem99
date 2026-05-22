using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem99.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Author { get; set; }

        [StringLength(50)]
        public string ISBN { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        public int TotalCopies { get; set; }

        [Required]
        public int AvailableCopies { get; set; }

        public string Description { get; set; }

        [Display(Name = "Cover Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string CoverImageUrl { get; set; }

        public virtual ICollection<BorrowingTransaction> BorrowingTransactions { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}