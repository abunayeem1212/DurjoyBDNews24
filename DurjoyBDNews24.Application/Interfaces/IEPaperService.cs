using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.EPaper;

namespace DurjoyBDNews24.Application.Interfaces;

public interface IEPaperService
{
    Task<PagedResult<EPaperDto>> GetPagedAsync(
        int page, int pageSize);
    Task<EPaperDto?> GetByIdAsync(int id);
    Task<EPaperDto?> GetTodayAsync();
    Task<IEnumerable<EPaperDto>> GetRecentAsync(int count = 7);
    Task<EPaperDto> CreateAsync(CreateEPaperDto dto);
    Task DeleteAsync(int id);
    Task IncrementViewAsync(int id);
    Task IncrementDownloadAsync(int id);
}