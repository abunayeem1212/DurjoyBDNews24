using DurjoyBDNews24.Application.DTOs.Payment;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Enums;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DurjoyBDNews24.Application.Services;

public class PaymentService(
    IUnitOfWork uow,
    IConfiguration config,
    ILogger<PaymentService> logger,
    HttpClient http) : IPaymentService
{
    private static readonly Dictionary<SubscriptionPlan, decimal> _prices = new()
    {
        { SubscriptionPlan.Basic,   99m  },
        { SubscriptionPlan.Premium, 199m }
    };

    public async Task<PaymentResponseDto> InitiateSubscriptionAsync(
        InitiatePaymentDto dto, string userId)
    {
        var amount = _prices.GetValueOrDefault(dto.Plan, 99m);
        var tranId = $"DBN_{userId}_{DateTime.UtcNow.Ticks}";
        var appUrl = config["AppUrl"]!;

        var payload = new Dictionary<string, string>
        {
            ["store_id"] = config["SSLCommerz:StoreId"]!,
            ["store_passwd"] = config["SSLCommerz:StorePass"]!,
            ["total_amount"] = amount.ToString("F2"),
            ["currency"] = "BDT",
            ["tran_id"] = tranId,
            ["success_url"] = $"{appUrl}/payment/success",
            ["fail_url"] = $"{appUrl}/payment/fail",
            ["cancel_url"] = $"{appUrl}/payment/cancel",
            ["cus_name"] = dto.UserName,
            ["cus_email"] = dto.Email,
            ["cus_phone"] = dto.Phone,
            ["cus_add1"] = "Bangladesh",
            ["cus_city"] = "Dhaka",
            ["cus_country"] = "Bangladesh",
            ["shipping_method"] = "NO",
            ["product_name"] = $"DurjoyBDNews24 {dto.Plan} Subscription",
            ["product_category"] = "Subscription",
            ["product_profile"] = "non-physical-goods"
        };

        var subscription = new Subscription
        {
            UserId = userId,
            Plan = dto.Plan,
            Amount = amount,
            TransactionId = tranId,
            PaymentMethod = "SSLCommerz",
            Status = PaymentStatus.Pending,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1)
        };

        await uow.Subscriptions.AddAsync(subscription);
        await uow.SaveChangesAsync();

        try
        {
            var response = await http.PostAsync(
                config["SSLCommerz:ApiUrl"],
                new FormUrlEncodedContent(payload));

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            return new PaymentResponseDto
            {
                GatewayUrl = result.GetProperty("GatewayPageURL").GetString()!,
                TransactionId = tranId
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SSLCommerz initiate failed for {TranId}", tranId);
            throw new InvalidOperationException("পেমেন্ট শুরু করা যায়নি। আবার চেষ্টা করুন।");
        }
    }

    public async Task<bool> HandleSuccessCallbackAsync(SslCommerzCallbackDto dto)
    {
        var subscription = await uow.Subscriptions
            .FindFirstAsync(s => s.TransactionId == dto.Tran_id);

        if (subscription is null)
        {
            logger.LogWarning("Subscription not found: {TranId}", dto.Tran_id);
            return false;
        }

        subscription.Status = PaymentStatus.Success;
        await uow.Subscriptions.UpdateAsync(subscription);

        var user = await uow.Users.GetByIdStringAsync(subscription.UserId);
        if (user is not null)
        {
            user.Plan = subscription.Plan;
            user.SubscriptionExpiry = subscription.EndDate;
            await uow.Users.UpdateUserAsync(user);
        }

        await uow.SaveChangesAsync();
        logger.LogInformation("Payment success: {TranId}", dto.Tran_id);
        return true;
    }

    public async Task<bool> HandleFailCallbackAsync(string tranId)
    {
        var subscription = await uow.Subscriptions
            .FindFirstAsync(s => s.TransactionId == tranId);

        if (subscription is null) return false;

        subscription.Status = PaymentStatus.Failed;
        await uow.Subscriptions.UpdateAsync(subscription);
        await uow.SaveChangesAsync();
        return true;
    }

    public async Task<SubscriptionDto?> GetActiveSubscriptionAsync(string userId)
    {
        var subscription = await uow.Subscriptions
            .FindFirstAsync(s =>
                s.UserId == userId &&
                s.Status == PaymentStatus.Success &&
                s.EndDate > DateTime.UtcNow);

        if (subscription is null) return null;

        return new SubscriptionDto
        {
            Id = subscription.Id,
            PlanName = subscription.Plan.ToString(),
            Amount = subscription.Amount,
            Status = subscription.Status.ToString(),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate
        };
    }
}