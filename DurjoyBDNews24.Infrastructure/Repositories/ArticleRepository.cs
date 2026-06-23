using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class ArticleRepository(ApplicationDbContext ctx)
    : BaseRepository<Article>(ctx), IArticleRepository
{
    public async Task<Article?> GetBySlugAsync(string slug) =>
        await _ctx.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Slug == slug && a.IsPublished);


    public async Task<Article?> GetByIdWithDetailsAsync(int id) =>
    await _ctx.Articles
        .Include(a => a.Category)
        .Include(a => a.Author)
        .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
        .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

    public async Task<PagedQueryResult<Article>> GetPagedAsync(
        int page, int pageSize,
        int? categoryId = null, bool? isPublished = null,
        bool? isBreaking = null, bool? isFeatured = null,
        string? searchTerm = null)
    {
        var query = _ctx.Articles
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Author)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(a => a.CategoryId == categoryId.Value);
        if (isPublished.HasValue)
            query = query.Where(a => a.IsPublished == isPublished.Value);
        if (isBreaking.HasValue)
            query = query.Where(a => a.IsBreaking == isBreaking.Value);
        if (isFeatured.HasValue)
            query = query.Where(a => a.IsFeatured == isFeatured.Value);
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(a =>
                a.Title.Contains(searchTerm) ||
                a.TitleBn.Contains(searchTerm) ||
                a.Summary!.Contains(searchTerm));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedQueryResult<Article>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<IEnumerable<Article>> GetBreakingNewsAsync(int count = 10) =>
        await _ctx.Articles
            .AsNoTracking()
            .Where(a => a.IsBreaking && a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Article>> GetFeaturedAsync(int count = 6) =>
        await _ctx.Articles
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => a.IsFeatured && a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Article>> GetByCategoryAsync(
        string categorySlug, int page, int pageSize) =>
        await _ctx.Articles
            .AsNoTracking()
            .Include(a => a.Author)
            .Where(a => a.Category.Slug == categorySlug && a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<IEnumerable<Article>> GetRelatedAsync(
        int articleId, int categoryId, int count = 5) =>
        await _ctx.Articles
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => a.CategoryId == categoryId
                     && a.Id != articleId
                     && a.IsPublished)
            .OrderByDescending(a => a.ViewCount)
            .Take(count)
            .ToListAsync();

    public async Task IncrementViewCountAsync(int articleId)
    {
        await _ctx.Articles
            .Where(a => a.Id == articleId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(a => a.ViewCount, a => a.ViewCount + 1));
    }

    public async Task<IEnumerable<Article>> SearchAsync(
        string query, int page, int pageSize) =>
        await _ctx.Articles
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Where(a => a.IsPublished && (
                a.Title.Contains(query) ||
                a.TitleBn.Contains(query) ||
                a.Summary!.Contains(query)))
            .OrderByDescending(a => a.ViewCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


    public async Task IncrementShareCountAsync(int articleId)
    {
        await _ctx.Articles
            .Where(a => a.Id == articleId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(a => a.ShareCount,
                              a => a.ShareCount + 1));
    }
}