using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TripPlaceDto
    {
        public int Id { get; init; }
        public PlacesResultDTO Places { get; init; }
        public DateTime VisitDate { get; init; }
        public int VisitOrder { get; init; }
        public TimeSpan? StartTime { get; init; }
        public TimeSpan? EndTime { get; init; }
        public decimal EstimatedCost { get; init; }
        public string Notes { get; init; }
        public bool IsCompleted { get; init; }
    }
}
