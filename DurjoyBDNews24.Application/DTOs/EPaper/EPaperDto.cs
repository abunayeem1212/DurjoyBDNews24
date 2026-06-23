namespace DurjoyBDNews24.Application.DTOs.EPaper;

public class EPaperDto
{
    public int Id { get; set; }
    public string TitleBn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PdfUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public bool IsPremiumOnly { get; set; }
    public long DownloadCount { get; set; }
    public long ViewCount { get; set; }
    public string? Edition { get; set; }
    public string PublishDateBn =>
        PublishDate.ToString("dd MMMM yyyy");
}

public class CreateEPaperDto
{
    public string TitleBn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PdfUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public DateTime PublishDate { get; set; } = DateTime.Today;
    public bool IsPublished { get; set; } = true;
    public bool IsPremiumOnly { get; set; } = false;
    public string? Edition { get; set; }
}