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
    public class UserConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            // الـ Budgets (decimal(10,2) في الداتابيز)
            builder.Property(u => u.DailyBudget)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2)
                .IsRequired(false); // Checked

            builder.Property(u => u.TotalBudget)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2)
                .IsRequired(false); // Checked
        }
    }
}
