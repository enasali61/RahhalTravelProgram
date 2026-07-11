using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.TripAndPlaces
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
        public string VisitingTimeEn { get; set; }
        public string? VisitingTimeAr { get; set; }

        [Column("CityEn")]
        public string CityEn { get; set; }
        public string CityAr { get; set; }
        [Column("Rating")]
        public decimal? Rating { get; set; } // Nullable لأنها Checked

        [Column("HistoricalBackGroundEn")]
        public string? HistoricalBackGroundEn { get; set; }
        [Column("HistoricalBackGroundAr")]
        public string? HistoricalBackGroundAr { get; set; }

        // Navigation Property - ممكن virtual أو لا
        public virtual Category? Category { get; set; }  
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
            
        // Foreign Key
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

       public virtual ICollection<UserPlaces> SavedByUsers { get; set; } = new List<UserPlaces>();
        public virtual ICollection<PlaceImages> Images { get; set; } = new List<PlaceImages>();
    }
}
