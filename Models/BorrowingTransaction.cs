using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem99.Models
{
    public class BorrowingTransaction
    {
        public int Id { get; set; }

        // =========================
        // FOREIGN KEYS
        // =========================

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        // =========================
        // BORROW INFO
        // =========================

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        // =========================
        // STATUS FLAGS
        // =========================

        public bool IsReturned { get; set; } = false;

        public int RenewalCount { get; set; } = 0;

        // =========================
        // NAVIGATION PROPERTIES
        // =========================

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // =========================
        // HELPER PROPERTIES (VERY USEFUL FOR DASHBOARD)
        // =========================

        [NotMapped]
        public bool IsOverdue
        {
            get
            {
                return !IsReturned && DateTime.Now > DueDate;
            }
        }

        [NotMapped]
        public int OverdueDays
        {
            get
            {
                if (!IsOverdue) return 0;
                return (DateTime.Now - DueDate).Days;
            }
        }
    }
}