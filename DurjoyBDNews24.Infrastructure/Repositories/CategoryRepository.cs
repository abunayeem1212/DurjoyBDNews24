using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext ctx)
    : BaseRepository<Category>(ctx), ICategoryRepository
{
    public async Task<Category?> GetBySlugAsync(string slug) =>
        await _ctx.Categories
            .Include(c => c.Children.Where(ch => ch.IsActive && !ch.IsDeleted))
            .FirstOrDefaultAsync(c => c.Slug == slug
                                   && c.IsActive
                                   && !c.IsDeleted);

    public async Task<IEnumerable<Category>> GetActiveWithChildrenAsync() =>
        await _ctx.Categories
            .AsNoTracking()
            .Include(c => c.Children.Where(ch => ch.IsActive && !ch.IsDeleted))
            .Where(c => c.IsActive && !c.IsDeleted && c.ParentId == null)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

    public async Task<IEnumerable<Category>> GetTopLevelAsync() =>
        await _ctx.Categories
            .AsNoTracking()
            .Where(c => c.ParentId == null
                     && c.IsActive
                     && !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
}