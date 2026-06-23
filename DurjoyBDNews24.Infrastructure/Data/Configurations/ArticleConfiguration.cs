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
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired().HasMaxLength(500);

            builder.Property(a => a.TitleBn)
                .IsRequired().HasMaxLength(500);

            builder.Property(a => a.Slug)
                .IsRequired().HasMaxLength(600);

            builder.HasIndex(a => a.Slug).IsUnique();
            builder.HasIndex(a => a.IsPublished);
            builder.HasIndex(a => a.IsBreaking);
            builder.HasIndex(a => a.IsFeatured);
            builder.HasIndex(a => a.PublishedAt);
            builder.HasIndex(a => a.CategoryId);

            builder.Property(a => a.Content)
                .IsRequired().HasColumnType("nvarchar(max)");

            builder.Property(a => a.ContentBn)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.MetaTitle)
                .HasMaxLength(160);

            builder.Property(a => a.MetaDescription)
                .HasMaxLength(320);

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
