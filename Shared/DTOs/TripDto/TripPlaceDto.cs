using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs.PlacesDto;

namespace Shared.DTOs.TripDto
{
    public class TripPlaceDto
    {
        public int Id { get; set; }
        public PlacesResultDTO Place { get; set; }
        public DateTime VisitDate { get; set; }
        public int VisitOrder { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Notes { get; set; }
        public bool IsCompleted { get; set; }
    }
}
