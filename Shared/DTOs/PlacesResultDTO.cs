using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DTOs
{
    public record PlacesResultDTO
    {
        public int Id { get; init; }
        public string NameAr { get; init; }
        public string NameEn { get; init; }

        // كل الأسعار
        public decimal PriceEgStudent { get; init; }
        public decimal PriceEgAdult { get; init; }
        public decimal PriceForeignStudent { get; init; }
        public decimal PriceForeignAdult { get; init; }
        public string Location { get; init; }
        public string Category { get; init; }
        public decimal? Rating { get; init; }
        public string Description { get; init; }

        [NotMapped]
        public string MainImageUrl { get; init; }

        [NotMapped]
        public List<string> GalleryImages { get; init; } = new();

        [NotMapped]
        public bool IsFavorite { get; init; }

    }
}
