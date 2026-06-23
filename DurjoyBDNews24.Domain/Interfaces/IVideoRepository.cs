using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface IVideoRepository
    : IBaseRepository<VideoNews>
{
    Task<IEnumerable<VideoNews>> GetFeaturedAsync(int count);
    Task<(IEnumerable<VideoNews> Items, int Total)>
        GetPagedAsync(int page, int pageSize);
    Task IncrementViewAsync(int id);
}

public interface ILiveTVRepository
    : IBaseRepository<LiveTV>
{
    Task<IEnumerable<LiveTV>> GetAllActiveAsync();
    Task<LiveTV?> GetLiveChannelAsync();
}