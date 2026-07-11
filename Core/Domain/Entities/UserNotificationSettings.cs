using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserNotificationSettings : BaseEntity
    {
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual Users User { get; set; }

        public bool TripReminders { get; set; } = true;
        public bool LiveArrivalAlerts { get; set; } = true;
        public bool ServiceDisruptions { get; set; } = true;
    }
}
