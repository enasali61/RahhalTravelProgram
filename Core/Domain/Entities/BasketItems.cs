using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BasketItems
    {
        public int Id { get; set; }  // unique per item
        public string Type { get; set; } // "Place" or "Trip"
        public int EntityId { get; set; } // PlaceId or TripId
        public int Quantity { get; set; } = 1;  // usually 1 for both
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PreferredDate { get; set; }
        public decimal? PriceSnapshot { get; set; } // optional, to freeze price at add time
    }
}
