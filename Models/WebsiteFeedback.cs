using System;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem99.Models
{
    public class WebsiteFeedback
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Your Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Your Feedback")]
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Whether the librarian has approved this for public display.
        // True by default so all submissions show; librarians can hide spam.
        public bool IsApproved { get; set; } = true;

        // Librarian's response
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Librarian Reply")]
        public string LibrarianReply { get; set; }

        public DateTime? RepliedDate { get; set; }
    }
}