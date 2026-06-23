using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.EPaper;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;

namespace DurjoyBDNews24.Application.Services;

public class EPaperService(IUnitOfWork uow) : IEPaperService
{
    public async Task<PagedResult<EPaperDto>> GetPagedAsync(
        int page, int pageSize)
    {
        var (items, total) = await uow.EPapers
            .GetPagedAsync(page, pageSize);

        return new PagedResult<EPaperDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<EPaperDto?> GetByIdAsync(int id)
    {
        var epaper = await uow.EPapers.GetByIdAsync(id);
        return epaper is null ? null : MapToDto(epaper);
    }

    public async Task<EPaperDto?> GetTodayAsync()
    {
        var epaper = await uow.EPapers.GetTodayAsync();
        return epaper is null ? null : MapToDto(epaper);
    }

    public async Task<IEnumerable<EPaperDto>> GetRecentAsync(
        int count = 7)
    {
        var epapers = await uow.EPapers.GetRecentAsync(count);
        return epapers.Select(MapToDto);
    }

    public async Task<EPaperDto> CreateAsync(CreateEPaperDto dto)
    {
        var epaper = new EPaper
        {
            TitleBn = dto.TitleBn,
            Title = dto.Title,
            PdfUrl = dto.PdfUrl,
            ThumbnailUrl = dto.ThumbnailUrl,
            PublishDate = dto.PublishDate,
            IsPublished = dto.IsPublished,
            IsPremiumOnly = dto.IsPremiumOnly,
            Edition = dto.Edition,
            CreatedAt = DateTime.UtcNow
        };

        await uow.EPapers.AddAsync(epaper);
        await uow.SaveChangesAsync();
        return MapToDto(epaper);
    }

    public async Task DeleteAsync(int id)
    {
        var epaper = await uow.EPapers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"E-Paper পাওয়া যায়নি: {id}");
        epaper.IsDeleted = true;
        await uow.EPapers.UpdateAsync(epaper);
        await uow.SaveChangesAsync();
    }

    public async Task IncrementViewAsync(int id) =>
        await uow.EPapers.IncrementViewAsync(id);

    public async Task IncrementDownloadAsync(int id) =>
        await uow.EPapers.IncrementDownloadAsync(id);

    private static EPaperDto MapToDto(EPaper e) => new()
    {
        Id = e.Id,
        TitleBn = e.TitleBn,
        Title = e.Title,
        PdfUrl = e.PdfUrl,
        ThumbnailUrl = e.ThumbnailUrl,
        PublishDate = e.PublishDate,
        IsPremiumOnly = e.IsPremiumOnly,
        DownloadCount = e.DownloadCount,
        ViewCount = e.ViewCount,
        Edition = e.Edition
    };
}