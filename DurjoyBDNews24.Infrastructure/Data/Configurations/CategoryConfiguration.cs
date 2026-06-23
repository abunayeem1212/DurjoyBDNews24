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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.NameBn).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Slug).IsRequired().HasMaxLength(200);
            builder.HasIndex(c => c.Slug).IsUnique();
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
