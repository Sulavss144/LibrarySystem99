using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem99.Models
{
    public class Fine
    {
        public int Id { get; set; }

        [Required]
        public int BorrowingTransactionId { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }

        public DateTime CreatedDate { get; set; }

        [ForeignKey("BorrowingTransactionId")]
        public virtual BorrowingTransaction BorrowingTransaction { get; set; }
    }
}