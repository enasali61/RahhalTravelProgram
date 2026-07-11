using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.TripDto
{
    public record UpdateTripDto
    {
       public string? TripName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TravelersCount { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}
