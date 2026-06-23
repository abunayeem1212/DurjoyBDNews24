using DurjoyBDNews24.Domain.Enums;

namespace DurjoyBDNews24.Application.DTOs.Payment;

public class InitiatePaymentDto
{
    public SubscriptionPlan Plan { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class PaymentResponseDto
{
    public string GatewayUrl { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}

public class SslCommerzCallbackDto
{
    public string Tran_id { get; set; } = string.Empty;
    public string Val_id { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Card_type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Store_amount { get; set; } = string.Empty;
}

public class SubscriptionDto
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive => EndDate > DateTime.UtcNow;
}

public class PressReleaseDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string ContentBn { get; set; } = string.Empty;
}