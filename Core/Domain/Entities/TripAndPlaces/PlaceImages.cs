using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.TripAndPlaces
{
    public class PlaceImages: BaseEntity
    {
        public int PlaceId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;

        [ForeignKey("PlaceId")]
        public virtual Places Place { get; set; }
    }
}
