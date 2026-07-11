using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.DTOs.TripDto
{
    public class TripDto
    {
        public int Id { get; set; }
        public string TripName { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal ActualSpent { get; set; }
        public int TravelersCount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        // Calculated properties
        public int DurationDays { get; set; }
        public decimal BudgetRemaining { get; set; }

        // Navigation
        public List<TripPlaceDto> TripPlaces { get; set; } = new();
        public int PlacesCount => TripPlaces?.Count ?? 0;
    }

    
}
