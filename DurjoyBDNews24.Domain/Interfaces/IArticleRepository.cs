using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface IArticleRepository : IBaseRepository<Article>
{
    Task<Article?> GetBySlugAsync(string slug);
    Task<Article?> GetByIdWithDetailsAsync(int id);

    Task IncrementShareCountAsync(int articleId);
    Task<PagedQueryResult<Article>> GetPagedAsync(
        int page, int pageSize,
        int? categoryId = null,
        bool? isPublished = null,
        bool? isBreaking = null,
        bool? isFeatured = null,
        string? searchTerm = null);
    Task<IEnumerable<Article>> GetBreakingNewsAsync(int count = 10);
    Task<IEnumerable<Article>> GetFeaturedAsync(int count = 6);
    Task<IEnumerable<Article>> GetByCategoryAsync(
        string categorySlug, int page, int pageSize);
    Task<IEnumerable<Article>> GetRelatedAsync(
        int articleId, int categoryId, int count = 5);
    Task IncrementViewCountAsync(int articleId);
    Task<IEnumerable<Article>> SearchAsync(string query, int page, int pageSize);
}