using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities.TripAndPlaces
{
    public class Trip : BaseEntity
    {
        [MaxLength(200)]
        public string TripName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsTemplate { get; set; } = true;   // true = ready‑made trips, false = user’s own trip 
        public decimal TotalBudget { get; set; } // total cost
        public decimal ActualSpent { get; set; } = 0;
        public int TravelersCount { get; set; } = 1;

        [MaxLength(20)]
        public TripStatusOption Status { get; set; } = TripStatusOption.Planned;

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
