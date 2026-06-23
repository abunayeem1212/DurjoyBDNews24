using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DurjoyBDNews24.Application.Services;

public class NewsletterService(
    IUnitOfWork uow,
    IEmailService emailService,
    ILogger<NewsletterService> logger) : INewsletterService
{
    public async Task<bool> SubscribeAsync(
        string email, string? name)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var existing = await uow.Newsletter
            .GetByEmailAsync(email);

        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await uow.Newsletter.UpdateAsync(existing);
                await uow.SaveChangesAsync();
            }
            return true;
        }

        var subscriber = new NewsletterSubscriber
        {
            Email = email,
            Name = name,
            IsActive = true,
            SubscribedAt = DateTime.UtcNow
        };

        await uow.Newsletter.AddAsync(subscriber);
        await uow.SaveChangesAsync();

        await emailService.SendAsync(
            email,
            "দুর্জয় বিডি নিউজ ২৪ — সাবস্ক্রিপশন নিশ্চিত",
            $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
                <div style="background:#c0392b;padding:20px;text-align:center;">
                    <h1 style="color:white;font-size:20px;margin:0;">
                        দুর্জয় বিডি নিউজ ২৪
                    </h1>
                </div>
                <div style="padding:24px;background:white;">
                    <h2 style="font-size:18px;color:#1a1a1a;">
                        সাবস্ক্রিপশন নিশ্চিত হয়েছে!
                    </h2>
                    <p style="font-size:15px;color:#555;line-height:1.7;">
                        ধন্যবাদ! আপনি সফলভাবে আমাদের নিউজলেটার
                        সাবস্ক্রাইব করেছেন।
                    </p>
                    <a href="#"
                       style="display:inline-block;margin-top:16px;font-size:13px;color:#888;">
                        আনসাবস্ক্রাইব করতে এখানে ক্লিক করুন
                    </a>
                </div>
            </div>
            """);

        logger.LogInformation(
            "Newsletter subscribed: {Email}", email);
        return true;
    }

    public async Task<bool> UnsubscribeAsync(string token)
    {
        var subscriber = await uow.Newsletter
            .GetByTokenAsync(token);
        if (subscriber is null) return false;

        subscriber.IsActive = false;
        await uow.Newsletter.UpdateAsync(subscriber);
        await uow.SaveChangesAsync();
        return true;
    }

    public async Task SendToAllAsync(
        string subject, string htmlBody)
    {
        var subscribers = await uow.Newsletter.GetActiveAsync();
        var count = 0;

        foreach (var sub in subscribers)
        {
            await emailService.SendAsync(
                sub.Email, subject, htmlBody);
            count++;
            await Task.Delay(100);
        }

        logger.LogInformation(
            "Newsletter sent to {Count} subscribers", count);
    }
}