using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class StatisticsController(
    IArticleService articleService,
    ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var all = await articleService.GetAllForAdminAsync(1, 1000);
        var categories = await categoryService.GetAllWithChildrenAsync();

        var topArticles = all.Items
            .OrderByDescending(a => a.ViewCount)
            .Take(10)
            .ToList();

        var categoryStats = all.Items
            .GroupBy(a => a.CategoryNameBn)
            .Select(g => new {
                Name = g.Key,
                Count = g.Count(),
                Views = g.Sum(a => a.ViewCount)
            })
            .OrderByDescending(x => x.Views)
            .ToList();

        var dailyStats = all.Items
            .Where(a => a.PublishedAt.HasValue &&
                   a.PublishedAt >= DateTime.UtcNow.AddDays(-30))
            .GroupBy(a => a.PublishedAt!.Value.Date)
            .Select(g => new {
                Date = g.Key.ToString("dd MMM"),
                Count = g.Count(),
                Views = g.Sum(a => a.ViewCount)
            })
            .OrderBy(x => x.Date)
            .ToList();

        ViewBag.TopArticles = topArticles;
        ViewBag.CategoryStats = categoryStats;
        ViewBag.DailyStats = dailyStats;
        ViewBag.TotalViews = all.Items.Sum(a => a.ViewCount);
        ViewBag.TotalArticles = all.TotalCount;
        ViewBag.TodayArticles = all.Items
            .Count(a => a.PublishedAt?.Date == DateTime.Today);

        return View();
    }
}