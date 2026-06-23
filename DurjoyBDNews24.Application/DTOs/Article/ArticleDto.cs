using System;
using System.Collections.Generic;

namespace DurjoyBDNews24.Application.DTOs.Article
{
    public class ArticleDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string TitleBn { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string? Summary { get; set; }
        public string? SummaryBn { get; set; }
        public string? ThumbnailUrl { get; set; }


        public string? CreatedBy { get; set; }

        public bool IsBreaking { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string? Content { get; set; }
        public string? ContentBn { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public long ViewCount { get; set; }
        public DateTime? PublishedAt { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public string CategoryNameBn { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;

        public string AuthorName { get; set; } = string.Empty;
        public string AuthorNameBn { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

    public class ArticleDetailDto : ArticleDto
    {
        public string Content { get; set; } = string.Empty;
        public string ContentBn { get; set; } = string.Empty;

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public int ShareCount { get; set; }
        public int CommentCount { get; set; }

        public List<ArticleDto> RelatedArticles { get; set; } = new();
    }

    public class CreateArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string TitleBn { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public string ContentBn { get; set; } = string.Empty;

        public string? Summary { get; set; }
        public string? SummaryBn { get; set; }
        public string? ThumbnailUrl { get; set; }

        public bool IsBreaking { get; set; }
        public bool IsFeatured { get; set; }

        public int CategoryId { get; set; }

        public List<int> TagIds { get; set; } = new();

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public DateTime? ScheduledAt { get; set; }
    }

    public class UpdateArticleDto : CreateArticleDto
    {
        public int Id { get; set; }
        public bool IsPublished { get; set; }
    }
}