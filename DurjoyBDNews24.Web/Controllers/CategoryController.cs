using DurjoyBDNews24.Web.Models;
using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Web.Controllers;

public class CategoryController(IApiService api) : BaseController(api)
{
    public async Task<IActionResult> Index(string slug, [FromQuery] int page = 1)
    {
        try
        {
            var categories = await api.GetCategoriesAsync();
            var category = categories.FirstOrDefault(c => c.Slug == slug);

            if (category is null)
                return RedirectToAction("Index", "Home");

            var articles = await api.GetByCategoryAsync(slug, page, 20);
            var breaking = await api.GetBreakingNewsAsync();

            var vm = new CategoryViewModel
            {
                CategoryName = category.NameBn,
                CategorySlug = slug,
                Articles = articles,
                Categories = categories,
                BreakingNews = breaking,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(articles.Count() / 20.0)
            };

            return View(vm);
        }
        catch
        {
            return RedirectToAction("Index", "Home");
        }
    }
}