using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.TripDto
{
    public record UpdateTripPlaceDto
    {
        public DateTime? VisitDate { get; set; }
        public int? VisitOrder { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? Notes { get; set; }
        public bool ?IsVisited { get; set; }

    }
}
