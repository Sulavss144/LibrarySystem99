using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem99.Models
{
    public class BorrowingTransaction
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; }

        public int RenewalCount { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}