using DurjoyBDNews24.Application.DTOs.Payment;
using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> InitiateSubscriptionAsync(
        InitiatePaymentDto dto, string userId);
    Task<bool> HandleSuccessCallbackAsync(SslCommerzCallbackDto dto);
    Task<bool> HandleFailCallbackAsync(string tranId);
    Task<SubscriptionDto?> GetActiveSubscriptionAsync(string userId);
}

public interface IAdService
{
    Task<Advertisement?> GetActiveAdForZoneAsync(int zoneId);
    Task RecordImpressionAsync(int adId);
    Task RecordClickAsync(int adId, string? userId, string ip);
}