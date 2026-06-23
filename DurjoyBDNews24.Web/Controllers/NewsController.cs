using DurjoyBDNews24.Web.Models;
using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Web.Controllers;

public class NewsController(IApiService api) : BaseController(api)
{
    public async Task<IActionResult> Detail(string slug)
    {
        var article = await api.GetBySlugAsync(slug);
        if (article is null) return NotFound();

        var vm = new ArticleDetailViewModel
        {
            Article = article,
            Categories = await api.GetCategoriesAsync(),
            BreakingNews = await api.GetBreakingNewsAsync()
        };

        return View(vm);
    }

    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1)
    {
        if (string.IsNullOrWhiteSpace(q))
            return RedirectToAction("Index", "Home");

        var result = await api.SearchAsync(q, page, 20);

        var vm = new SearchViewModel
        {
            Query = q,
            Articles = result.Items,
            Categories = await api.GetCategoriesAsync(),
            TotalCount = result.TotalCount,
            CurrentPage = page,
            TotalPages = result.TotalPages
        };

        return View(vm);
    }


}