using DurjoyBDNews24.Application.DTOs.Video;

namespace DurjoyBDNews24.Application.Interfaces;

public interface ILiveTVService
{
    Task<IEnumerable<LiveTVDto>> GetAllActiveAsync();
    Task<LiveTVDto?> GetByIdAsync(int id);
    Task<LiveTVDto> CreateAsync(CreateLiveTVDto dto);
    Task UpdateAsync(int id, CreateLiveTVDto dto);
    Task DeleteAsync(int id);
    Task SetLiveAsync(int id, bool isLive);
}