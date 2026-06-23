namespace DurjoyBDNews24.Application.Interfaces;

public interface IEmailService
{
    Task SendBreakingNewsEmailAsync(
        string to, string title, string slug);
    Task SendWelcomeEmailAsync(
        string to, string nameBn);
    Task SendAsync(
        string to, string subject, string htmlBody);
}