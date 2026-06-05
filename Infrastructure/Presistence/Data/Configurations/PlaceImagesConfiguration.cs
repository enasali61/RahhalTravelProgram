using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class PlaceImagesConfiguration : IEntityTypeConfiguration<PlaceImages>
    {
        public void Configure(EntityTypeBuilder<PlaceImages> builder)
        {
            builder.ToTable("PlaceImages");
            
            builder.HasOne(p => p.Place)
                .WithMany(pl => pl.Images)
                .HasForeignKey(p => p.PlaceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

           

        } 
    }
}
