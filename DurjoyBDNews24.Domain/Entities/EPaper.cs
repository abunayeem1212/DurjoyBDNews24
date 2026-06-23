using DurjoyBDNews24.Domain.Common;

namespace DurjoyBDNews24.Domain.Entities;

public class EPaper : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string PdfUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public DateTime PublishDate { get; set; } = DateTime.Today;
    public bool IsPublished { get; set; } = true;
    public bool IsPremiumOnly { get; set; } = false;
    public long DownloadCount { get; set; } = 0;
    public long ViewCount { get; set; } = 0;
    public string? Edition { get; set; }
}