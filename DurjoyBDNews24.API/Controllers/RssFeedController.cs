using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;

namespace DurjoyBDNews24.API.Controllers;

[ApiController]
[Route("")]
public class RssFeedController(
    IArticleService articleService) : ControllerBase
{
    [HttpGet("rss.xml")]
    public async Task<IActionResult> Rss()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var articles = await articleService.GetPagedAsync(1, 50);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\">");
        sb.AppendLine("<channel>");
        sb.AppendLine("<title>দুর্জয় বিডি নিউজ ২৪</title>");
        sb.AppendLine($"<link>{baseUrl}</link>");
        sb.AppendLine("<description>সত্যের সন্ধানে নির্ভীক</description>");
        sb.AppendLine("<language>bn</language>");
        sb.AppendLine($"<lastBuildDate>{DateTime.UtcNow:R}</lastBuildDate>");
        sb.AppendLine($"<atom:link href=\"{baseUrl}/rss.xml\" rel=\"self\" type=\"application/rss+xml\"/>");

        foreach (var article in articles.Items)
        {
            var pubDate = article.PublishedAt?.ToString("R")
                ?? DateTime.UtcNow.ToString("R");
            var summary = article.SummaryBn ?? article.TitleBn;
            var encodedSummary = System.Security.SecurityElement
                .Escape(summary);
            var encodedTitle = System.Security.SecurityElement
                .Escape(article.TitleBn);

            sb.AppendLine("<item>");
            sb.AppendLine($"<title>{encodedTitle}</title>");
            sb.AppendLine($"<link>{baseUrl}/news/{article.Slug}</link>");
            sb.AppendLine($"<guid>{baseUrl}/news/{article.Slug}</guid>");
            sb.AppendLine($"<pubDate>{pubDate}</pubDate>");
            sb.AppendLine($"<category>{article.CategoryNameBn}</category>");
            sb.AppendLine($"<description>{encodedSummary}</description>");

            if (!string.IsNullOrEmpty(article.ThumbnailUrl))
            {
                sb.AppendLine($"<enclosure url=\"{article.ThumbnailUrl}\" type=\"image/jpeg\"/>");
            }

            sb.AppendLine("</item>");
        }

        sb.AppendLine("</channel>");
        sb.AppendLine("</rss>");

        return Content(sb.ToString(), "application/rss+xml", Encoding.UTF8);
    }

    [HttpGet("rss/{categorySlug}.xml")]
    public async Task<IActionResult> CategoryRss(string categorySlug)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var articles = await articleService
            .GetByCategoryAsync(categorySlug, 1, 20);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<rss version=\"2.0\">");
        sb.AppendLine("<channel>");
        sb.AppendLine($"<title>দুর্জয় বিডি নিউজ ২৪ — {categorySlug}</title>");
        sb.AppendLine($"<link>{baseUrl}/category/{categorySlug}</link>");
        sb.AppendLine("<language>bn</language>");

        foreach (var article in articles)
        {
            sb.AppendLine("<item>");
            sb.AppendLine($"<title>{System.Security.SecurityElement.Escape(article.TitleBn)}</title>");
            sb.AppendLine($"<link>{baseUrl}/news/{article.Slug}</link>");
            sb.AppendLine($"<pubDate>{article.PublishedAt?.ToString("R")}</pubDate>");
            sb.AppendLine("</item>");
        }

        sb.AppendLine("</channel>");
        sb.AppendLine("</rss>");

        return Content(sb.ToString(), "application/rss+xml", Encoding.UTF8);
    }
}