using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.UserDto
{
    public record UserResultDTO(string DisplayName, string Email, string Token);
    public record UserProfileDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int CompletedTripsCount { get; set; }
        public int VisitedPlacesCount { get; set; }
        public int SavedPlacesCount { get; set; }
        public List<RecentTripDto> RecentTrips { get; set; }

    }
    public record AdminProfileDto 
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int TotalUsersCount { get; set; }       // Card 1: Users (e.g., 1,248)
        public int TotalPlacesCount { get; set; }      // Card 2: Places (e.g., 156)
        public int TotalTripPlansCount { get; set; }   // Card 3: Trip Plans (e.g., 24)
        public int TotalScansCount { get; set; }
    }
    public class AdminPlaceGridItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }               // Display name based on screen language
        public string CategoryName { get; set; }       // e.g., "Historical"
        public string City { get; set; }               // e.g., "Giza"
        public string ImageUrl { get; set; }

        // Pricing matrices displayed on Screen 23
        public decimal PriceEgAdult { get; set; }
        public decimal PriceForeignAdult { get; set; }
        public decimal PriceEgStudent { get; set; }
    }
    public class ToggleLocationSharingDto
    {
        public bool Enabled { get; set; }
    }
    public class UpdateLanguageDto
    {
        public string Language { get; set; } // "en" or "ar"
    }
    public class UpdateNotificationSettingsDto
    {
        public bool? TripReminders { get; set; }
        public bool? LiveArrivalAlerts { get; set; }
        public bool? ServiceDisruptions { get; set; }
    }
    public class NotificationSettingsDto
    {
        public int Id { get; set; }
        public bool TripReminders { get; set; }
        public bool LiveArrivalAlerts { get; set; }
        public bool ServiceDisruptions { get; set; }
    }
    public record RecentTripDto
    {
        public int Id { get; set; }
        public string TripName { get; set; }
        public DateTime StartDate { get; set; }
        public string? MainImageUrl { get; set; } 
    }
}
