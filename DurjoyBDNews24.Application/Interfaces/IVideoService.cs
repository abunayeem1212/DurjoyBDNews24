using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.Video;

namespace DurjoyBDNews24.Application.Interfaces;

public interface IVideoService
{
    Task<PagedResult<VideoNewsDto>> GetPagedAsync(
        int page, int pageSize);
    Task<VideoNewsDto?> GetByIdAsync(int id);
    Task<IEnumerable<VideoNewsDto>> GetFeaturedAsync(int count = 6);
    Task<VideoNewsDto> CreateAsync(
        CreateVideoNewsDto dto, string authorId);
    Task DeleteAsync(int id);
    Task IncrementViewAsync(int id);
}

