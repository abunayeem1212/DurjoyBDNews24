using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface IEPaperRepository
    : IBaseRepository<EPaper>
{
    Task<EPaper?> GetTodayAsync();
    Task<IEnumerable<EPaper>> GetRecentAsync(int count);
    Task<(IEnumerable<EPaper> Items, int Total)>
        GetPagedAsync(int page, int pageSize);
    Task IncrementViewAsync(int id);
    Task IncrementDownloadAsync(int id);
}