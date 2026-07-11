using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SubEntity
{
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; set; }          // "weekly" ,"Monthly", "Yearly"
        public string StripePriceId { get; set; } // from Stripe dashboard
        public decimal Price { get; set; }
        public int DurationWeeks { get; set; }   // 1,4,52
        public string FeaturesJson { get; set; }  // e.g., ["AI Recommendations", "Unlimited Trips planning"]
    }
}
