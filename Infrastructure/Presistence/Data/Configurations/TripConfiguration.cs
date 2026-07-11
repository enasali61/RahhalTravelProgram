using Domain.Entities.TripAndPlaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            // Decimal Properties
            builder.Property(t => t.TotalBudget)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            builder.Property(t => t.ActualSpent)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            builder.Property(o => o.Status)
              .HasConversion(
              s => s.ToString(),
              s => Enum.Parse<TripStatusOption>(s));

            // Relationships
            builder.HasOne(t => t.User)
                .WithMany(u => u.Trips)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
