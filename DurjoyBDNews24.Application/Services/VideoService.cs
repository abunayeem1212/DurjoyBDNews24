using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.Video;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace DurjoyBDNews24.Application.Services;

public class VideoService(IUnitOfWork uow) : IVideoService
{
    public async Task<PagedResult<VideoNewsDto>> GetPagedAsync(
        int page, int pageSize)
    {
        var (items, total) = await uow.Videos
            .GetPagedAsync(page, pageSize);

        return new PagedResult<VideoNewsDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<VideoNewsDto?> GetByIdAsync(int id)
    {
        var video = await uow.Videos.GetByIdAsync(id);
        return video is null ? null : MapToDto(video);
    }

    public async Task<IEnumerable<VideoNewsDto>>
        GetFeaturedAsync(int count = 6)
    {
        var videos = await uow.Videos.GetFeaturedAsync(count);
        return videos.Select(MapToDto);
    }

    public async Task<VideoNewsDto> CreateAsync(
        CreateVideoNewsDto dto, string authorId)
    {
        var youtubeId = ExtractYoutubeId(dto.YoutubeUrl);
        if (string.IsNullOrEmpty(youtubeId))
            throw new InvalidOperationException(
                "সঠিক YouTube URL দিন");

        var thumbnail = !string.IsNullOrEmpty(youtubeId)
            ? $"https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg"
            : null;

        var video = new VideoNews
        {
            TitleBn = dto.TitleBn,
            Title = dto.Title,
            DescriptionBn = dto.DescriptionBn,
            YoutubeUrl = dto.YoutubeUrl,
            YoutubeId = youtubeId,
            ThumbnailUrl = thumbnail,
            IsFeatured = dto.IsFeatured,
            IsPublished = dto.IsPublished,
            CategoryId = dto.CategoryId,
            AuthorId = authorId,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Videos.AddAsync(video);
        await uow.SaveChangesAsync();
        return MapToDto(video);
    }

    public async Task DeleteAsync(int id)
    {
        var video = await uow.Videos.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"ভিডিও পাওয়া যায়নি: {id}");
        video.IsDeleted = true;
        await uow.Videos.UpdateAsync(video);
        await uow.SaveChangesAsync();
    }

    public async Task IncrementViewAsync(int id) =>
        await uow.Videos.IncrementViewAsync(id);

    private static string ExtractYoutubeId(string url)
    {
        var patterns = new[]
        {
            @"youtube\.com/watch\?v=([^&\s]+)",
            @"youtu\.be/([^?\s]+)",
            @"youtube\.com/embed/([^?\s]+)",
            @"youtube\.com/shorts/([^?\s]+)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(url, pattern);
            if (match.Success)
                return match.Groups[1].Value;
        }
        return string.Empty;
    }

    private static VideoNewsDto MapToDto(VideoNews v) => new()
    {
        Id = v.Id,
        TitleBn = v.TitleBn,
        Title = v.Title,
        DescriptionBn = v.DescriptionBn,
        YoutubeId = v.YoutubeId,
        YoutubeUrl = v.YoutubeUrl,
        ThumbnailUrl = v.ThumbnailUrl,
        IsFeatured = v.IsFeatured,
        ViewCount = v.ViewCount,
        PublishedAt = v.PublishedAt,
        CategoryNameBn = v.Category?.NameBn ?? "",
        AuthorNameBn = v.Author?.FullNameBn ?? ""
    };
}

public class LiveTVService(IUnitOfWork uow) : ILiveTVService
{
    public async Task<IEnumerable<LiveTVDto>> GetAllActiveAsync()
    {
        var channels = await uow.LiveTV.GetAllActiveAsync();
        return channels.Select(MapToDto);
    }

    public async Task<LiveTVDto?> GetByIdAsync(int id)
    {
        var channel = await uow.LiveTV.GetByIdAsync(id);
        return channel is null ? null : MapToDto(channel);
    }

    public async Task<LiveTVDto> CreateAsync(CreateLiveTVDto dto)
    {
        var channel = new LiveTV
        {
            NameBn = dto.NameBn,
            Name = dto.Name,
            Description = dto.Description,
            StreamUrl = dto.StreamUrl,
            YoutubeStreamId = dto.YoutubeStreamId,
            ThumbnailUrl = dto.ThumbnailUrl,
            SortOrder = dto.SortOrder,
            IsActive = true,
            IsLive = false,
            CreatedAt = DateTime.UtcNow
        };

        await uow.LiveTV.AddAsync(channel);
        await uow.SaveChangesAsync();
        return MapToDto(channel);
    }

    public async Task UpdateAsync(int id, CreateLiveTVDto dto)
    {
        var channel = await uow.LiveTV.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"Channel পাওয়া যায়নি: {id}");

        channel.NameBn = dto.NameBn;
        channel.Name = dto.Name;
        channel.Description = dto.Description;
        channel.StreamUrl = dto.StreamUrl;
        channel.YoutubeStreamId = dto.YoutubeStreamId;
        channel.ThumbnailUrl = dto.ThumbnailUrl;
        channel.SortOrder = dto.SortOrder;
        channel.UpdatedAt = DateTime.UtcNow;

        await uow.LiveTV.UpdateAsync(channel);
        await uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var channel = await uow.LiveTV.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"Channel পাওয়া যায়নি: {id}");
        channel.IsDeleted = true;
        await uow.LiveTV.UpdateAsync(channel);
        await uow.SaveChangesAsync();
    }

    public async Task SetLiveAsync(int id, bool isLive)
    {
        if (isLive)
        {
            var allChannels = await uow.LiveTV.GetAllActiveAsync();
            foreach (var ch in allChannels.Where(c => c.IsLive))
            {
                ch.IsLive = false;
                await uow.LiveTV.UpdateAsync(ch);
            }
        }

        var channel = await uow.LiveTV.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"Channel পাওয়া যায়নি: {id}");
        channel.IsLive = isLive;
        await uow.LiveTV.UpdateAsync(channel);
        await uow.SaveChangesAsync();
    }

    private static LiveTVDto MapToDto(LiveTV t) => new()
    {
        Id = t.Id,
        NameBn = t.NameBn,
        Name = t.Name,
        Description = t.Description,
        StreamUrl = t.StreamUrl,
        YoutubeStreamId = t.YoutubeStreamId,
        ThumbnailUrl = t.ThumbnailUrl,
        IsLive = t.IsLive,
        IsActive = t.IsActive,
        SortOrder = t.SortOrder
    };
}