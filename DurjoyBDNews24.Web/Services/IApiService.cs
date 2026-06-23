using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Auth;
using DurjoyBDNews24.Application.DTOs.Category;
using DurjoyBDNews24.Application.DTOs.Comment;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.EPaper;
using DurjoyBDNews24.Application.DTOs.Payment;
using DurjoyBDNews24.Application.DTOs.Video;

namespace DurjoyBDNews24.Web.Services;

public interface IApiService
{
    Task<IEnumerable<ArticleDto>> GetBreakingNewsAsync();
    Task<IEnumerable<ArticleDto>> GetFeaturedAsync();
    Task<PagedResult<ArticleDto>> GetPagedAsync(int page, int pageSize, int? categoryId = null);
    Task<ArticleDetailDto?> GetBySlugAsync(string slug);
    Task<IEnumerable<ArticleDto>> GetByCategoryAsync(string slug, int page, int pageSize);
    Task<PagedResult<ArticleDto>> SearchAsync(string q, int page, int pageSize);
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();


    Task<PagedResult<EPaperDto>> GetEPapersAsync(int page, int pageSize);
    Task<EPaperDto?> GetTodayEPaperAsync();
    Task<IEnumerable<EPaperDto>> GetRecentEPapersAsync();

    Task<PagedResult<VideoNewsDto>> GetVideosAsync(
    int page, int pageSize);
    Task<VideoNewsDto?> GetVideoByIdAsync(int id);
    Task<IEnumerable<VideoNewsDto>> GetFeaturedVideosAsync();
    Task<IEnumerable<LiveTVDto>> GetLiveTVChannelsAsync();

    Task<AuthResponseDto> LoginAsync(string email, string password);
    Task<AuthResponseDto> RegisterAsync(
        string fullNameBn, string email,
        string password, string phone);


    Task<SubscriptionDto?> GetMySubscriptionAsync(string token);
    Task<IEnumerable<CommentDto>> GetMyCommentsAsync(string token);
}