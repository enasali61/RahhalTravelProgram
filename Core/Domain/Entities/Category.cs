using Domain.Entities.TripAndPlaces;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public virtual ICollection<Places> Places { get; set; } = new List<Places>();
    }
}
