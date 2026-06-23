using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DurjoyBDNews24.API.Controllers;

[ApiController]
[Route("")]
public class SitemapController(
    IArticleService articleService,
    ICategoryService categoryService) : ControllerBase
{
    [HttpGet("sitemap.xml")]
    public async Task<IActionResult> Sitemap()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var sb = new StringBuilder();

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        sb.AppendLine($"""
            <url>
                <loc>{baseUrl}</loc>
                <changefreq>hourly</changefreq>
                <priority>1.0</priority>
            </url>
        """);

        var categories = await categoryService.GetAllWithChildrenAsync();
        foreach (var cat in categories)
        {
            sb.AppendLine($"""
                <url>
                    <loc>{baseUrl}/category/{cat.Slug}</loc>
                    <changefreq>hourly</changefreq>
                    <priority>0.8</priority>
                </url>
            """);
        }

        var articles = await articleService.GetPagedAsync(1, 1000);
        foreach (var article in articles.Items)
        {
            sb.AppendLine($"""
                <url>
                    <loc>{baseUrl}/news/{article.Slug}</loc>
                    <lastmod>{article.PublishedAt:yyyy-MM-dd}</lastmod>
                    <changefreq>weekly</changefreq>
                    <priority>0.6</priority>
                </url>
            """);
        }

        sb.AppendLine("</urlset>");

        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }

    [HttpGet("robots.txt")]
    public IActionResult Robots()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var content = $"""
            User-agent: *
            Allow: /
            Disallow: /api/
            Disallow: /hangfire/
            Disallow: /health/
            Sitemap: {baseUrl}/sitemap.xml
        """;
        return Content(content, "text/plain");
    }
}