using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.TripAndPlaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class TripPlaceConfiguration : IEntityTypeConfiguration<TripPlace>
    {
        public void Configure(EntityTypeBuilder<TripPlace> builder)
        {
            builder.HasKey(up => new { up.TripId, up.PlaceId });
            // Decimal Properties
            builder.Property(tp => tp.EstimatedCost)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            // Relationships
            builder.HasOne(tp => tp.Trip)
                .WithMany(t => t.TripPlaces)
                .HasForeignKey(tp => tp.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tp => tp.Place)
                .WithMany()
                .HasForeignKey(tp => tp.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
