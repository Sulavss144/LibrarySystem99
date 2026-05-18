using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibrarySystem99.Models
{
    public class ApplicationUser : IdentityUser
    {
        // ===== EXTENDED PROFILE FIELDS =====

        [StringLength(150)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(250)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(500)]
        [Display(Name = "Photo URL")]
        public string PhotoUrl { get; set; }

        [StringLength(1000)]
        [Display(Name = "Bio")]
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowingTransaction> BorrowingTransactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<BorrowingPolicy> BorrowingPolicies { get; set; }

        public DbSet<WebsiteFeedback> WebsiteFeedbacks { get; set; }

        public DbSet<BookReview> BookReviews { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    public static class PolicyHelper
    {
        // Default fallbacks if no policy is configured yet
        public const int DefaultMaxBooksPerUser = 3;
        public const int DefaultBorrowDays = 14;
        public const int DefaultMaxRenewals = 2;
        public const decimal DefaultFinePerDay = 1.00m;

        public static BorrowingPolicy GetActivePolicy(ApplicationDbContext db)
        {
            // Use the first policy in the DB; fall back to defaults if none exists
            var policy = db.BorrowingPolicies.FirstOrDefault();
            if (policy != null) return policy;

            return new BorrowingPolicy
            {
                MaxBooksPerUser = DefaultMaxBooksPerUser,
                BorrowDays = DefaultBorrowDays,
                MaxRenewals = DefaultMaxRenewals,
                FinePerDay = DefaultFinePerDay
            };
        }

        public static void EnsureDefaultPolicyExists(ApplicationDbContext db)
        {
            if (!db.BorrowingPolicies.Any())
            {
                db.BorrowingPolicies.Add(new BorrowingPolicy
                {
                    MaxBooksPerUser = DefaultMaxBooksPerUser,
                    BorrowDays = DefaultBorrowDays,
                    MaxRenewals = DefaultMaxRenewals,
                    FinePerDay = DefaultFinePerDay
                });
                db.SaveChanges();
            }
        }
    }
}