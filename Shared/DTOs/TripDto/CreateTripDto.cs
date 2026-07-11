using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.TripDto
{
    public record CreateTripDto
    {
        public string TripName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TravelersCount { get; set; } = 1;
        public decimal TotalBudget { get; set; }
        public string? Notes { get; set; }
    }
}
