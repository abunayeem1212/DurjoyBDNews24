using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId);
    Task<IEnumerable<Comment>> GetPendingAsync();
    Task<int> GetCountByArticleAsync(int articleId);
}