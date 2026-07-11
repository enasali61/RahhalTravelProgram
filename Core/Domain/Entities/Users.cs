using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Domain.Entities.SubEntity;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class Users : IdentityUser<int>
    {
       
        [Column("User_type")]
        [MaxLength(50)]
        public string UserType { get; set; } = "User"; // "Admin", "User"

        public bool IsEgyptian { get; set; } = true;  // or false for foreign
        public bool IsStudent { get; set; } = false;

        [Column("InterestsJson")]
        public string InterestsJson { get; set; } = "[]";

        [Column("DailyBudget")]
        public decimal? DailyBudget { get; set; }

        [Column("TotalBudget")]
        public decimal? TotalBudget { get; set; }
        public string? StripeCustomerId { get; set; }
        public string Language { get; set; } = "en"; 
        public bool LiveLocationSharing { get; set; } = false;

        [Column("TravelGroup")]
        [MaxLength(50)]
        public string TravelGroup { get; set; } = "solo";// "Solo", "Family", "Friends"  //////////////////////////////////////// edit for number of people in the group

        // Navigation properties
        public virtual ICollection<UserPlaces> SavedPlaces { get; set; } = new List<UserPlaces>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
        public virtual ICollection<UserNotificationSettings> NotificationSettings { get; set; } = new List<UserNotificationSettings>();

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