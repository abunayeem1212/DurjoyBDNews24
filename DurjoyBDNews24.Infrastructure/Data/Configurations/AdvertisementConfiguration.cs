using DurjoyBDNews24.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Infrastructure.Data.Configurations
{
    public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title).IsRequired().HasMaxLength(300);
            builder.Property(a => a.TargetUrl).IsRequired().HasMaxLength(1000);
            builder.Property(a => a.DailyRate).HasColumnType("decimal(10,2)");
            builder.HasIndex(a => a.IsApproved);
            builder.HasIndex(a => new { a.StartDate, a.EndDate });
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
