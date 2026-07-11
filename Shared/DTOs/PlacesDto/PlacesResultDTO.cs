using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DTOs.PlacesDto
{
    public record PlacesResultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VisitingTime { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public decimal? Rating { get; set; }
        public string HistoricalBackGround { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [NotMapped]
        public string MainImageUrl { get; set; }

        [NotMapped]
        public List<string> GalleryImages { get; set; } = new();

        [NotMapped]
        public bool IsFavorite { get; set; }

    }
}
