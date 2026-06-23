using DurjoyBDNews24.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace DurjoyBDNews24.Infrastructure.Services;

public class EmailService(
    IConfiguration config,
    ILogger<EmailService> logger) : IEmailService
{
    public async Task SendBreakingNewsEmailAsync(
        string to, string title, string slug)
    {
        var siteUrl = config["AppUrl"];
        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
                <div style="background:#c0392b;padding:20px;text-align:center;">
                    <h1 style="color:white;font-size:20px;margin:0;">
                        দুর্জয় বিডি নিউজ ২৪
                    </h1>
                </div>
                <div style="padding:24px;background:white;">
                    <div style="background:#fde8e8;border-left:4px solid #c0392b;padding:12px 16px;margin-bottom:20px;">
                        <strong style="color:#c0392b;">⚡ ব্রেকিং নিউজ</strong>
                    </div>
                    <h2 style="font-size:20px;color:#1a1a1a;line-height:1.4;margin:0 0 16px;">
                        {title}
                    </h2>
                    <a href="{siteUrl}/news/{slug}"
                       style="display:inline-block;padding:12px 24px;background:#c0392b;color:white;text-decoration:none;border-radius:6px;font-size:14px;">
                        বিস্তারিত পড়ুন
                    </a>
                </div>
                <div style="background:#f5f5f5;padding:16px;text-align:center;font-size:12px;color:#888;">
                    দুর্জয় বিডি নিউজ ২৪
                </div>
            </div>
        """;
        await SendAsync(to, $"ব্রেকিং: {title}", html);
    }

    public async Task SendWelcomeEmailAsync(
        string to, string nameBn)
    {
        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
                <div style="background:#c0392b;padding:20px;text-align:center;">
                    <h1 style="color:white;font-size:20px;margin:0;">
                        দুর্জয় বিডি নিউজ ২৪
                    </h1>
                </div>
                <div style="padding:24px;background:white;">
                    <h2 style="font-size:20px;color:#1a1a1a;">
                        স্বাগতম, {nameBn}!
                    </h2>
                    <p style="font-size:15px;color:#555;line-height:1.7;">
                        দুর্জয় বিডি নিউজ ২৪ পরিবারে আপনাকে স্বাগত।
                    </p>
                </div>
            </div>
        """;
        await SendAsync(to, "দুর্জয় বিডি নিউজ ২৪ — স্বাগতম!", html);
    }

    public async Task SendAsync(
        string to, string subject, string htmlBody)
    {
        try
        {
            var smtpHost = config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(config["Email:SmtpPort"] ?? "587");
            var smtpUser = config["Email:SmtpUser"] ?? "";
            var smtpPass = config["Email:SmtpPass"] ?? "";
            var fromEmail = config["Email:FromEmail"] ?? smtpUser;
            var fromName = config["Email:FromName"]
                ?? "দুর্জয় বিডি নিউজ ২৪";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(to);
            await client.SendMailAsync(message);
            logger.LogInformation(
                "Email sent to {To}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Email failed to {To}: {Subject}", to, subject);
        }
    }
}