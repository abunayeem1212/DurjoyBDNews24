namespace DurjoyBDNews24.Application.DTOs.Video;

public class VideoNewsDto
{
    public int Id { get; set; }
    public string TitleBn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? DescriptionBn { get; set; }
    public string YoutubeId { get; set; } = string.Empty;
    public string YoutubeUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public bool IsFeatured { get; set; }
    public long ViewCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string CategoryNameBn { get; set; } = string.Empty;
    public string AuthorNameBn { get; set; } = string.Empty;

    public string EmbedUrl =>
        $"https://www.youtube.com/embed/{YoutubeId}?autoplay=0&rel=0";
    public string WatchUrl =>
        $"https://www.youtube.com/watch?v={YoutubeId}";
}

public class CreateVideoNewsDto
{
    public string TitleBn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? DescriptionBn { get; set; }
    public string YoutubeUrl { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; } = true;
    public int? CategoryId { get; set; }
}

public class LiveTVDto
{
    public int Id { get; set; }
    public string NameBn { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StreamUrl { get; set; } = string.Empty;
    public string? YoutubeStreamId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsLive { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }

    public string EmbedUrl => !string.IsNullOrEmpty(YoutubeStreamId)
        ? $"https://www.youtube.com/embed/{YoutubeStreamId}?autoplay=1"
        : StreamUrl;
}

public class CreateLiveTVDto
{
    public string NameBn { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StreamUrl { get; set; } = string.Empty;
    public string? YoutubeStreamId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int SortOrder { get; set; }
}