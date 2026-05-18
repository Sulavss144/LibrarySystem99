using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem99.Models
{
    public enum ReservationStatus
    {
        Waiting = 0,    // Member is in the queue, book not yet available
        Ready = 1,      // Book has been returned, ready for member to pick up
        Fulfilled = 2,  // Member borrowed the book
        Cancelled = 3   // Member or librarian cancelled
    }

    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        public DateTime? ReadyDate { get; set; }

        public DateTime? FulfilledDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Waiting;

        // Navigation properties
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}