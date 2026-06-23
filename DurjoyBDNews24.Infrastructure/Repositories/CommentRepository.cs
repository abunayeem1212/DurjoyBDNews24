using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class CommentRepository(ApplicationDbContext ctx)
    : BaseRepository<Comment>(ctx), ICommentRepository
{
    public async Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId) =>
        await _ctx.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Replies).ThenInclude(r => r.User)
            .Where(c => c.ArticleId == articleId
                     && c.IsApproved
                     && c.ParentId == null
                     && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Comment>> GetPendingAsync() =>
        await _ctx.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Article)
            .Where(c => !c.IsApproved && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

    public async Task<int> GetCountByArticleAsync(int articleId) =>
        await _ctx.Comments
            .CountAsync(c => c.ArticleId == articleId
                          && c.IsApproved
                          && !c.IsDeleted);
}