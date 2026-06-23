using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class VideoRepository(ApplicationDbContext ctx)
    : BaseRepository<VideoNews>(ctx), IVideoRepository
{
    public async Task<IEnumerable<VideoNews>> GetFeaturedAsync(
        int count) =>
        await _ctx.VideoNews
            .AsNoTracking()
            .Include(v => v.Category)
            .Include(v => v.Author)
            .Where(v => v.IsPublished
                     && v.IsFeatured
                     && !v.IsDeleted)
            .OrderByDescending(v => v.PublishedAt)
            .Take(count)
            .ToListAsync();

    public async Task<(IEnumerable<VideoNews> Items, int Total)>
        GetPagedAsync(int page, int pageSize)
    {
        var query = _ctx.VideoNews
            .AsNoTracking()
            .Include(v => v.Category)
            .Include(v => v.Author)
            .Where(v => v.IsPublished && !v.IsDeleted)
            .OrderByDescending(v => v.PublishedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task IncrementViewAsync(int id) =>
        await _ctx.VideoNews
            .Where(v => v.Id == id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(v => v.ViewCount,
                              v => v.ViewCount + 1));
}

public class LiveTVRepository(ApplicationDbContext ctx)
    : BaseRepository<LiveTV>(ctx), ILiveTVRepository
{
    public async Task<IEnumerable<LiveTV>> GetAllActiveAsync() =>
        await _ctx.LiveTVChannels
            .AsNoTracking()
            .Where(t => t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

    public async Task<LiveTV?> GetLiveChannelAsync() =>
        await _ctx.LiveTVChannels
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.IsLive && t.IsActive && !t.IsDeleted);
}