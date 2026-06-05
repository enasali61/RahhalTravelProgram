using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Places : BaseEntity
    {

        [Column("name_ar")]
        public string NameAr { get; set; }

        [Column("name_en")]
        public string NameEn { get; set; }

        [Column("price_eg_student")]
        public decimal PriceEgStudent { get; set; }

        [Column("price_eg_adult")]
        public decimal PriceEgAdult { get; set; }

        [Column("price_forign_adult")]
        public decimal PriceForeignAdult { get; set; }

        [Column("price_forign_student")]
        public decimal PriceForeignStudent { get; set; } 

        [Column("Location")]
        public string Location { get; set; }
        
        [Column("Rating")]
        public decimal? Rating { get; set; } // Nullable لأنها Checked

        [Column("Description")]
        public string? Description { get; set; }
      
        // Navigation Property - ممكن virtual أو لا
        public virtual Category? Category { get; set; }      
        // Foreign Key
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

       public virtual ICollection<UserPlaces> SavedByUsers { get; set; } = new List<UserPlaces>();
        public virtual ICollection<PlaceImages> Images { get; set; } = new List<PlaceImages>();
    }
}
