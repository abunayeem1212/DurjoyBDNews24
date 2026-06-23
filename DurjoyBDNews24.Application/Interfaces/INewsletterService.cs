namespace DurjoyBDNews24.Application.Interfaces;

public interface INewsletterService
{
    Task<bool> SubscribeAsync(string email, string? name);
    Task<bool> UnsubscribeAsync(string token);
    Task SendToAllAsync(string subject, string htmlBody);
}