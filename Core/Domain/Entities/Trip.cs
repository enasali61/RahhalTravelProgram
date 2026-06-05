using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Trip : BaseEntity
    {
        [MaxLength(200)]
        public string TripName { get; set; } // "رحلة أسوان ٣ أيام"        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalBudget { get; set; }
        public decimal ActualSpent { get; set; } = 0;
        public int TravelersCount { get; set; } = 1;

        [MaxLength(20)]
        public string Status { get; set; } = "planned"; // planned, ongoing, completed, cancelled

        [Column("notes")]
        public string? Notes { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public virtual Users User { get; set; }

        // Navigation Properties
        public virtual ICollection<TripPlace> TripPlaces { get; set; } = new List<TripPlace>();

        // Helper Properties (NotMapped)
        [NotMapped]
        public int DurationDays => (EndDate - StartDate).Days + 1;

        [NotMapped]
        public decimal BudgetRemaining => TotalBudget - ActualSpent;
    }
}
