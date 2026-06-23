using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Common;

namespace DurjoyBDNews24.Application.Interfaces;

public interface IArticleService
{
    Task<PagedResult<ArticleDto>> GetPagedAsync(
        int page, int pageSize,
        int? categoryId = null,
        string? searchTerm = null);
    Task<PagedResult<ArticleDto>> GetAllForAdminAsync(
        int page, int pageSize,
        string? searchTerm = null);
    Task<ArticleDetailDto?> GetBySlugAsync(string slug);
    Task<ArticleDetailDto?> GetByIdAsync(int id);
    Task<IEnumerable<ArticleDto>> GetBreakingNewsAsync(int count = 10);
    Task<IEnumerable<ArticleDto>> GetFeaturedAsync(int count = 6);
    Task<IEnumerable<ArticleDto>> GetByCategoryAsync(
        string slug, int page, int pageSize);
    Task<PagedResult<ArticleDto>> SearchAsync(
        string query, int page, int pageSize);
    Task<ArticleDetailDto> CreateAsync(CreateArticleDto dto, string authorId);
    Task<ArticleDetailDto> UpdateAsync(UpdateArticleDto dto, string editorId);
    Task DeleteAsync(int id);
    Task PublishAsync(int id);
    Task UnpublishAsync(int id);
    Task SetBreakingAsync(int id, bool isBreaking);
    Task SetFeaturedAsync(int id, bool isFeatured);
}