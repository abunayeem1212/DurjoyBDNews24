using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Category;

namespace DurjoyBDNews24.Web.Models;

public class HomeViewModel
{
    public IEnumerable<ArticleDto> BreakingNews { get; set; } = [];
    public IEnumerable<ArticleDto> FeaturedArticles { get; set; } = [];
    public IEnumerable<ArticleDto> LatestArticles { get; set; } = [];
    public IEnumerable<CategoryDto> Categories { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class ArticleDetailViewModel
{
    public ArticleDetailDto Article { get; set; } = null!;
    public IEnumerable<CategoryDto> Categories { get; set; } = [];
    public IEnumerable<ArticleDto> BreakingNews { get; set; } = [];
}

public class CategoryViewModel
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public IEnumerable<ArticleDto> Articles { get; set; } = [];
    public IEnumerable<CategoryDto> Categories { get; set; } = [];
    public IEnumerable<ArticleDto> BreakingNews { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class SearchViewModel
{
    public string Query { get; set; } = string.Empty;
    public IEnumerable<ArticleDto> Articles { get; set; } = [];
    public IEnumerable<CategoryDto> Categories { get; set; } = [];
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}