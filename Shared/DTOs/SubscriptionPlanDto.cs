using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string DurationDisplay { get; set; } // ✅ Add this for UI
        public List<string> Features { get; set; }
        public string StripePriceId { get; set; }
    }
}
