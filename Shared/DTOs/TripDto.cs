using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TripDto
    {
        public int Id { get; init; }
        public string TripName { get; init; }
        public int UserId { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal TotalBudget { get; init; }
        public decimal ActualSpent { get; init; }
        public int TravelersCount { get; init; }
        public string Status { get; init; }
        public string Notes { get; init; }

        // Calculated properties
        public int DurationDays { get; init; }  
        public decimal BudgetRemaining { get; init; }

        // Navigation
        public List<TripPlaceDto> TripPlaces { get; init; } = new();
    }
}
