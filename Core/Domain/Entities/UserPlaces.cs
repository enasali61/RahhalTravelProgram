using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserPlaces
    {
        public int UserId { get; set; }
        public int PlaceId { get; set; }

        // Navigation properties
        public virtual Users User { get; set; }
        public virtual Places Place { get; set; }

    }
}
