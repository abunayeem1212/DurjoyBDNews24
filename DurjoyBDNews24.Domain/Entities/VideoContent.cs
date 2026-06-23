using DurjoyBDNews24.Domain.Common;

namespace DurjoyBDNews24.Domain.Entities;

public class VideoNews : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionBn { get; set; }
    public string YoutubeUrl { get; set; } = string.Empty;
    public string YoutubeId { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public bool IsPublished { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public long ViewCount { get; set; } = 0;
    public DateTime? PublishedAt { get; set; } = DateTime.UtcNow;
    public int? CategoryId { get; set; }
    public string? AuthorId { get; set; }
    public Category? Category { get; set; }
    public AppUser? Author { get; set; }
}

public class LiveTV : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StreamUrl { get; set; } = string.Empty;
    public string? YoutubeStreamId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsLive { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}