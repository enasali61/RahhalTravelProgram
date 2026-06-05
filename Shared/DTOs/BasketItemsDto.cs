using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record BasketItemsDto
    {
        public int Id { get; init; }                     // unique identifier for this basket item
        public string Type { get; init; }                   // "Place" or "Trip"
        public int EntityId { get; init; }                  // PlaceId or TripId
        public string Name { get; init; }                   // display name (e.g., place name or trip name)
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }              // optional, for UI
        public decimal? Price { get; init; }                // current or snapshot price
        public int Quantity { get; init; } = 1;
        public DateTime AddedAt { get; init; }
        public string? Notes { get; init; }
        
    }
}
