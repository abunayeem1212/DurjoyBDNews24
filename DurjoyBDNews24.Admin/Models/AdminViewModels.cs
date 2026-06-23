using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Category;

namespace DurjoyBDNews24.Admin.Models;

public class DashboardViewModel
{
    public int TotalArticles { get; set; }
    public int PublishedArticles { get; set; }
    public int DraftArticles { get; set; }
    public int TotalCategories { get; set; }
    public long TotalViews { get; set; }
    public IEnumerable<ArticleDto> RecentArticles { get; set; } = [];
}

public class ArticleListViewModel
{
    public IEnumerable<ArticleDto> Articles { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string? SearchTerm { get; set; }
}

public class ArticleFormViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentBn { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? SummaryBn { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsBreaking { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int CategoryId { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public IEnumerable<CategoryDto> Categories { get; set; } = [];
}

public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}