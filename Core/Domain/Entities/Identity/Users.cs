using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class Users : IdentityUser<int>
    {
        // 🔹 Remove these – they are already in IdentityUser:
        // public string Email { get; set; }        // IdentityUser has Email
        // public string Password { get; set; }     // IdentityUser has PasswordHash
        // public string Phone { get; set; }        // IdentityUser has PhoneNumber

        [Column("User_type")]
        [MaxLength(50)]
        public string UserType { get; set; } = "User"; // "Admin", "User", "TourGuide"

        [Column("InterestsJson")]
        public string InterestsJson { get; set; }

        [Column("DailyBudget")]
        public decimal? DailyBudget { get; set; }

        [Column("TotalBudget")]
        public decimal? TotalBudget { get; set; }

        [Column("TravelGroup")]
        [MaxLength(50)]
        public string TravelGroup { get; set; } // "Solo", "Family", "Friends"

        // Navigation properties
        public virtual ICollection<UserPlaces> SavedPlaces { get; set; } = new List<UserPlaces>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

        // Helper for Interests (unchanged)
        [NotMapped]
        public List<string> Interests
        {
            get => string.IsNullOrEmpty(InterestsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(InterestsJson);
            set => InterestsJson = JsonSerializer.Serialize(value);
        }
    }
}