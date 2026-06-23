using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DurjoyBDNews24.Domain.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string TitleBn { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ContentBn { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? SummaryBn { get; set; }
        public string? ThumbnailUrl { get; set; }

        public bool IsBreaking { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

        public long ViewCount { get; set; } = 0;
        public int ShareCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public DateTime? PublishedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }

        public int CategoryId { get; set; }
        public string AuthorId { get; set; } = string.Empty;

        public Category Category { get; set; } = null!;
        public AppUser Author { get; set; } = null!;
        public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    }
}
