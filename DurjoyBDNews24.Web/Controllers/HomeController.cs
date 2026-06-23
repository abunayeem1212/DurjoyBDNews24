using DurjoyBDNews24.Web.Models;
using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DurjoyBDNews24.Web.Controllers;

public class HomeController(
    IApiService api,
    IConfiguration config) : BaseController(api, config)
{
    public async Task<IActionResult> Index(
        [FromQuery] int page = 1,
        [FromQuery] string? date = null,
        [FromQuery] string? category = null)
    {
        var breaking = await api.GetBreakingNewsAsync();
        var featured = await api.GetFeaturedAsync();
        var categories = await api.GetCategoriesAsync();

        var paged = string.IsNullOrEmpty(category)
            ? await api.GetPagedAsync(page, 20)
            : await api.GetPagedAsync(page, 20);

        ViewBag.SelectedDate = date;
        ViewBag.SelectedCategory = category;

        var vm = new HomeViewModel
        {
            BreakingNews = breaking,
            FeaturedArticles = featured,
            LatestArticles = paged.Items,
            Categories = categories,
            CurrentPage = page,
            TotalPages = paged.TotalPages
        };

        return View(vm);
    }

    public IActionResult About() => View();

    [Route("Home/Error/{code?}")]
    public IActionResult Error(int? code)
    {
        ViewData["Title"] = code == 404
            ? "পাতাটি পাওয়া যায়নি"
            : "সমস্যা হয়েছে";
        return View("Error");
    }
}