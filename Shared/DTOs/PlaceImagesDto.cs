namespace Shared.DTOs
{
    public class PlaceImagesDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }
}
