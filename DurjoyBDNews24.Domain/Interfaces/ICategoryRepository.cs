using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetActiveWithChildrenAsync();
    Task<IEnumerable<Category>> GetTopLevelAsync();
}