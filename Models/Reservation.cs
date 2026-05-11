using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem99.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime ReservationDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsFulfilled { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}