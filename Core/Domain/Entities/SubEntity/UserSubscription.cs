using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SubEntity
{
    public class UserSubscription : BaseEntity
    {
        public int UserId { get; set; }
        public virtual Users User { get; set; }
        public int SubscriptionPlanId { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string StripeSubscriptionId { get; set; }
    }
}
