using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlacesDto
{
    public record CreatePlaceDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public decimal PriceEgStudent { get; set; }
        public decimal PriceEgAdult { get; set; }
        public decimal PriceForeignAdult { get; set; }
        public decimal PriceForeignStudent { get; set; }
        public string VisitingTime { get; set; }
        public string CityEn { get; set; }
        public string CityAr { get; set; }
        public decimal? Rating { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
