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
    public class PlacesConfiguration : IEntityTypeConfiguration<Places>
    {
        public void Configure(EntityTypeBuilder<Places> builder)
        {
            // الـ Prices (decimal(10,2) في الداتابيز)
            builder.Property(p => p.PriceEgStudent)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            builder.Property(p => p.PriceEgAdult)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            builder.Property(p => p.PriceForeignAdult)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            builder.Property(p => p.PriceForeignStudent)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            // Rating (decimal(3,2) في الداتابيز)
            builder.Property(p => p.Rating)
                .HasColumnType("decimal(3,2)")
                .HasPrecision(3, 2)
                .IsRequired(false); // لأنه Checked في الداتابيز
        }
    }
    
}
