using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record AddToBasketDto
    {
        public string Type { get; set; }   // "Place" or "Trip"
        public int EntityId { get; set; }  // PlaceId or TripId
    }
}
