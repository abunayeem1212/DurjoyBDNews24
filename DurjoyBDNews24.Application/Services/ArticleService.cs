using AutoMapper;
using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Enums;
using DurjoyBDNews24.Domain.Interfaces;

namespace DurjoyBDNews24.Application.Services;

public class ArticleService(
    IUnitOfWork uow,
    IMapper mapper,
    ISlugService slugService,
    ICacheService cache,
    INewsHubService? hubService = null) : IArticleService
{
    private const string BreakingCacheKey = "breaking_news";
    private const string FeaturedCacheKey = "featured_news";

    public async Task<PagedResult<ArticleDto>> GetAllForAdminAsync(
    int page, int pageSize, string? searchTerm = null)
    {
        var result = await uow.Articles.GetPagedAsync(
            page, pageSize,
            categoryId: null,
            isPublished: null,
            searchTerm: searchTerm);

        return new PagedResult<ArticleDto>
        {
            Items = mapper.Map<IEnumerable<ArticleDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ArticleDetailDto?> GetBySlugAsync(string slug)
    {
        var article = await uow.Articles.GetBySlugAsync(slug);
        if (article is null) return null;

        await uow.Articles.IncrementViewCountAsync(article.Id);

        if (hubService is not null)
            await hubService.BroadcastViewCountAsync(article.Id, article.ViewCount + 1);

        var dto = mapper.Map<ArticleDetailDto>(article);
        var related = await uow.Articles.GetRelatedAsync(article.Id, article.CategoryId);
        dto.RelatedArticles = mapper.Map<List<ArticleDto>>(related);

        return dto;
    }

    public async Task<IEnumerable<ArticleDto>> GetBreakingNewsAsync(int count = 10)
    {
        var cached = await cache.GetAsync<IEnumerable<ArticleDto>>(BreakingCacheKey);
        if (cached is not null) return cached;

        var articles = await uow.Articles.GetBreakingNewsAsync(count);
        var dto = mapper.Map<IEnumerable<ArticleDto>>(articles);

        await cache.SetAsync(BreakingCacheKey, dto, TimeSpan.FromMinutes(5));
        return dto;
    }

    public async Task<IEnumerable<ArticleDto>> GetFeaturedAsync(int count = 6)
    {
        var cached = await cache.GetAsync<IEnumerable<ArticleDto>>(FeaturedCacheKey);
        if (cached is not null) return cached;

        var articles = await uow.Articles.GetFeaturedAsync(count);
        var dto = mapper.Map<IEnumerable<ArticleDto>>(articles);

        await cache.SetAsync(FeaturedCacheKey, dto, TimeSpan.FromMinutes(10));
        return dto;
    }

    public async Task<IEnumerable<ArticleDto>> GetByCategoryAsync(
        string slug, int page, int pageSize)
    {
        var articles = await uow.Articles.GetByCategoryAsync(slug, page, pageSize);
        return mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<PagedResult<ArticleDto>> SearchAsync(
        string query, int page, int pageSize)
    {
        var articles = await uow.Articles.SearchAsync(query, page, pageSize);
        var total = await uow.Articles.CountAsync(a =>
            a.IsPublished && (
            a.Title.Contains(query) ||
            a.TitleBn.Contains(query)));

        return new PagedResult<ArticleDto>
        {
            Items = mapper.Map<IEnumerable<ArticleDto>>(articles),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ArticleDetailDto> CreateAsync(CreateArticleDto dto, string authorId)
    {
        var article = mapper.Map<Article>(dto);
        article.Slug = await slugService.GenerateUniqueAsync(dto.TitleBn);
        article.AuthorId = authorId;


        article.CreatedBy = authorId;
        article.Status = ArticleStatus.PendingReview;

        foreach (var tagId in dto.TagIds)
            article.ArticleTags.Add(new ArticleTag { TagId = tagId });

        await uow.Articles.AddAsync(article);
        await uow.SaveChangesAsync();

        var resultDto = mapper.Map<ArticleDetailDto>(article);

        if (hubService is not null)
            await hubService.BroadcastNewArticleAsync(
                mapper.Map<ArticleDto>(article),
                article.Category?.Slug ?? "");

        return resultDto;
    }

    public async Task<ArticleDetailDto> UpdateAsync(UpdateArticleDto dto, string editorId)
    {
        var article = await uow.Articles.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"নিউজ পাওয়া যায়নি: {dto.Id}");

        mapper.Map(dto, article);
        article.UpdatedBy = editorId;
        article.UpdatedAt = DateTime.UtcNow;

        await uow.Articles.UpdateAsync(article);
        await uow.SaveChangesAsync();
        await cache.RemoveByPatternAsync("breaking_");
        await cache.RemoveByPatternAsync("featured_");

        return mapper.Map<ArticleDetailDto>(article);
    }

    public async Task DeleteAsync(int id)
    {
        var article = await uow.Articles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"নিউজ পাওয়া যায়নি: {id}");
        article.IsDeleted = true;
        await uow.Articles.UpdateAsync(article);
        await uow.SaveChangesAsync();
    }

    public async Task PublishAsync(int id)
    {
        var article = await uow.Articles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"নিউজ পাওয়া যায়নি: {id}");

        article.IsPublished = true;
        article.Status = ArticleStatus.Published;
        article.PublishedAt = DateTime.UtcNow;

        await uow.Articles.UpdateAsync(article);
        await uow.SaveChangesAsync();
        await cache.RemoveAsync(FeaturedCacheKey);

        if (hubService is not null)
        {
            var dto = mapper.Map<ArticleDto>(article);
            await hubService.BroadcastNewArticleAsync(
                dto, article.Category?.Slug ?? "");
        }
    }

    public async Task UnpublishAsync(int id)
    {
        var article = await uow.Articles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"নিউজ পাওয়া যায়নি: {id}");
        article.IsPublished = false;
        article.Status = ArticleStatus.Archived;
        await uow.Articles.UpdateAsync(article);
        await uow.SaveChangesAsync();
    }

    public async Task SetBreakingAsync(int id, bool isBreaking)
    {
        var article = await uow.Articles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"নিউজ পাওয়া যায়নি: {id}");

        article.IsBreaking = isBreaking;
        await uow.Articles.UpdateAsync(article);
        await uow.SaveChangesAsync();
        await cache.RemoveAsync(BreakingCacheKey);

        if (isBreaking && hubService is not null)
        {
            var dto = mapper.Map<ArticleDto>(article);
            await hubService.BroadcastBreakingNewsAsync(dto);
        }
    }

    public async Task SetFeaturedAsync(int id, bool isFeatured)
    {
        var article = await uow.Articles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"নিউজ পাওয়া যায়নি: {id}");
        article.IsFeatured = isFeatured;
        await uow.Articles.UpdateAsync(article);
        await uow.SaveChangesAsync();
        await cache.RemoveAsync(FeaturedCacheKey);
    }

    public async Task<ArticleDetailDto?> GetByIdAsync(int id)
    {
        var article = await uow.Articles.GetByIdWithDetailsAsync(id);
        return article is null ? null : mapper.Map<ArticleDetailDto>(article);
    }


    //chatgpt
    public async Task<PagedResult<ArticleDto>> GetPagedAsync(
    int page,
    int pageSize,
    int? categoryId = null,
    string? searchTerm = null)
    {
        var result = await uow.Articles.GetPagedAsync(
            page,
            pageSize,
            categoryId: categoryId,
            isPublished: true, // public side এ শুধু published দেখাবে
            searchTerm: searchTerm);

        return new PagedResult<ArticleDto>
        {
            Items = mapper.Map<IEnumerable<ArticleDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}