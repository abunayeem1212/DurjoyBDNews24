using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Enums;

namespace DurjoyBDNews24.Domain.Entities;

public class Subscription : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; }
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public AppUser User { get; set; } = null!;
}

public class PressRelease : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentBn { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsPaid { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public string? TransactionId { get; set; }
}