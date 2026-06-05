using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class UserPlaceConfiguration : IEntityTypeConfiguration<UserPlaces>
    {
        public void Configure(EntityTypeBuilder<UserPlaces> builder)
        {
            builder.HasKey(up => new { up.UserId, up.PlaceId });
            
            builder.HasOne(up => up.User)
               .WithMany(u => u.SavedPlaces)
               .HasForeignKey(up => up.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Place)
                .WithMany(p => p.SavedByUsers)
                .HasForeignKey(up => up.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
