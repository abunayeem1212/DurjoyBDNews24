using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class EPaperRepository(ApplicationDbContext ctx)
    : BaseRepository<EPaper>(ctx), IEPaperRepository
{
    public async Task<EPaper?> GetTodayAsync() =>
        await _ctx.EPapers
            .AsNoTracking()
            .Where(e => e.PublishDate.Date == DateTime.Today
                     && e.IsPublished
                     && !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<EPaper>> GetRecentAsync(
        int count) =>
        await _ctx.EPapers
            .AsNoTracking()
            .Where(e => e.IsPublished && !e.IsDeleted)
            .OrderByDescending(e => e.PublishDate)
            .Take(count)
            .ToListAsync();

    public async Task<(IEnumerable<EPaper> Items, int Total)>
        GetPagedAsync(int page, int pageSize)
    {
        var query = _ctx.EPapers
            .AsNoTracking()
            .Where(e => e.IsPublished && !e.IsDeleted)
            .OrderByDescending(e => e.PublishDate);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task IncrementViewAsync(int id) =>
        await _ctx.EPapers
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(e => e.ViewCount,
                              e => e.ViewCount + 1));

    public async Task IncrementDownloadAsync(int id) =>
        await _ctx.EPapers
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(e => e.DownloadCount,
                              e => e.DownloadCount + 1));
}