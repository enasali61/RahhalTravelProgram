using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.TripAndPlaces
{
    public class TripPlace 
    {
        [ForeignKey("Trip")]
        public int TripId { get; set; }
        public virtual Trip Trip { get; set; }

        [ForeignKey("Places")]
        public int PlaceId { get; set; }
        public virtual Places Place { get; set; }

        public DateTime? VisitDate { get; set; } 
        public int? VisitOrder { get; set; } // ترتيب الزيارة في ذلك اليوم (1, 2, 3...)

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [MaxLength(20)]
        public string? TransportMode { get; set; } // walking, car, taxi, bus
        public decimal EstimatedCost { get; set; } // cost for this place * number of travelers يعني التكلفه الكامله في المكان دا بعدد الاشخاص دول
        public string? Notes { get; set; }
        public bool IsVisited { get; set; } = false;
    }
}
