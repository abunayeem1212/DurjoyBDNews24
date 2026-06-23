using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Admin.Models;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class DashboardController(
    IArticleService articleService,
    ICategoryService categoryService,
    ICommentService commentService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var allArticles = await articleService
            .GetAllForAdminAsync(1, 1000);
        var published = await articleService
            .GetPagedAsync(1, 1);
        var categories = await categoryService
            .GetAllWithChildrenAsync();
        var pendingComments = await commentService
            .GetPendingAsync();
        var recent = await articleService
            .GetAllForAdminAsync(1, 10);

        var vm = new DashboardViewModel
        {
            TotalArticles = allArticles.TotalCount,
            PublishedArticles = published.TotalCount,
            DraftArticles = allArticles.TotalCount
                - published.TotalCount,
            TotalCategories = categories.Count(),
            TotalViews = allArticles.Items
                .Sum(a => a.ViewCount),
            RecentArticles = recent.Items
        };

        ViewBag.PendingComments = pendingComments.Count();
        ViewBag.AdminName = HttpContext.Session
            .GetString("AdminName");
        ViewBag.AdminRole = HttpContext.Session
            .GetString("AdminRole");

        return View(vm);
    }
}