using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Auth;
using DurjoyBDNews24.Application.DTOs.Category;
using DurjoyBDNews24.Application.DTOs.Comment;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.EPaper;
using DurjoyBDNews24.Application.DTOs.Payment;
using DurjoyBDNews24.Application.DTOs.Video;
using System.Text.Json;

namespace DurjoyBDNews24.Web.Services;

public class ApiService(HttpClient http) : IApiService
{
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<T>>(url, _opts);
            return response is { Success: true } ? response.Data : default;
        }
        catch
        {
            return default;
        }
    }

    public async Task<IEnumerable<ArticleDto>> GetBreakingNewsAsync()
        => await GetAsync<IEnumerable<ArticleDto>>("news/breaking") ?? [];

    public async Task<IEnumerable<ArticleDto>> GetFeaturedAsync()
        => await GetAsync<IEnumerable<ArticleDto>>("news/featured") ?? [];

    public async Task<PagedResult<ArticleDto>> GetPagedAsync(
        int page, int pageSize, int? categoryId = null)
    {
        var url = $"news?page={page}&pageSize={pageSize}";
        if (categoryId.HasValue) url += $"&categoryId={categoryId}";
        return await GetAsync<PagedResult<ArticleDto>>(url)
            ?? new PagedResult<ArticleDto>();
    }

    public async Task<ArticleDetailDto?> GetBySlugAsync(string slug)
        => await GetAsync<ArticleDetailDto>($"news/{slug}");

    public async Task<IEnumerable<ArticleDto>> GetByCategoryAsync(
        string slug, int page, int pageSize)
        => await GetAsync<IEnumerable<ArticleDto>>(
            $"news/category/{slug}?page={page}&pageSize={pageSize}") ?? [];

    public async Task<PagedResult<ArticleDto>> SearchAsync(
        string q, int page, int pageSize)
        => await GetAsync<PagedResult<ArticleDto>>(
            $"news/search?q={Uri.EscapeDataString(q)}&page={page}&pageSize={pageSize}")
            ?? new PagedResult<ArticleDto>();

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        => await GetAsync<IEnumerable<CategoryDto>>("category") ?? [];


    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var res = await http.PostAsJsonAsync("auth/login", new { email, password });
        var data = await res.Content
            .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>(_opts);
        if (data is not { Success: true })
            throw new InvalidOperationException(data?.Message ?? "লগইন ব্যর্থ হয়েছে");
        return data.Data!;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        string fullNameBn, string email, string password, string phone)
    {
        var res = await http.PostAsJsonAsync("auth/register", new
        {
            fullName = fullNameBn,
            fullNameBn,
            email,
            password,
            phone
        });
        var data = await res.Content
            .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>(_opts);
        if (data is not { Success: true })
            throw new InvalidOperationException(
                data?.Message ?? "রেজিস্ট্রেশন ব্যর্থ হয়েছে");
        return data.Data!;
    }


    public async Task<SubscriptionDto?> GetMySubscriptionAsync(string token)
    {
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await GetAsync<SubscriptionDto>("payment/my-subscription");
    }

    public async Task<IEnumerable<CommentDto>> GetMyCommentsAsync(string token)
    {
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await GetAsync<IEnumerable<CommentDto>>("comment/my") ?? [];
    }


    public async Task<PagedResult<VideoNewsDto>> GetVideosAsync(
    int page, int pageSize)
    => await GetAsync<PagedResult<VideoNewsDto>>(
        $"video?page={page}&pageSize={pageSize}")
       ?? new PagedResult<VideoNewsDto>();

    public async Task<VideoNewsDto?> GetVideoByIdAsync(int id)
        => await GetAsync<VideoNewsDto>($"video/{id}");

    public async Task<IEnumerable<VideoNewsDto>>
        GetFeaturedVideosAsync()
        => await GetAsync<IEnumerable<VideoNewsDto>>(
            "video/featured") ?? [];

    public async Task<IEnumerable<LiveTVDto>>
        GetLiveTVChannelsAsync()
        => await GetAsync<IEnumerable<LiveTVDto>>(
            "livetv") ?? [];


    public async Task<PagedResult<EPaperDto>> GetEPapersAsync(
    int page, int pageSize)
    => await GetAsync<PagedResult<EPaperDto>>(
        $"epaper?page={page}&pageSize={pageSize}")
       ?? new PagedResult<EPaperDto>();

    public async Task<EPaperDto?> GetTodayEPaperAsync()
        => await GetAsync<EPaperDto>("epaper/today");

    public async Task<IEnumerable<EPaperDto>> GetRecentEPapersAsync()
        => await GetAsync<IEnumerable<EPaperDto>>("epaper/recent")
           ?? [];

}