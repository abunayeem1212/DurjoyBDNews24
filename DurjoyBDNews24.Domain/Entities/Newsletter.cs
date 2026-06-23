using DurjoyBDNews24.Domain.Common;

namespace DurjoyBDNews24.Domain.Entities;

public class NewsletterSubscriber : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool IsActive { get; set; } = true;
    public string UnsubscribeToken { get; set; } =
        Guid.NewGuid().ToString("N");
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}