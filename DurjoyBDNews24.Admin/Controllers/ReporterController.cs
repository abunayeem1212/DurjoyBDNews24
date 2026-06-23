using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Admin.Models;
using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class ReporterController(
    IArticleService articleService,
    ICategoryService categoryService) : Controller
{
    private string CurrentUserId =>
        HttpContext.Session.GetString("AdminUserId") ?? "";

    private string CurrentRole =>
        HttpContext.Session.GetString("AdminRole") ?? "";

    public async Task<IActionResult> Index([FromQuery] int page = 1)
    {
        var result = await articleService.GetAllForAdminAsync(page, 20);

        var articles = CurrentRole == "Reporter"
            ? result.Items.Where(a => a.CreatedBy == CurrentUserId)
            : result.Items;

        var vm = new ArticleListViewModel
        {
            Articles = articles,
            CurrentPage = page,
            TotalPages = result.TotalPages,
            TotalCount = articles.Count()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Write()
    {
        var categories = await categoryService.GetAllWithChildrenAsync();
        return View(new ArticleFormViewModel
        {
            Categories = categories
        });
    }
    [HttpPost]
    public async Task<IActionResult> Write(ArticleFormViewModel vm)
    {
        try
        {
            // ✅ Session থেকে UserId নেওয়া
            var authorId = HttpContext.Session.GetString("AdminUserId") ?? "";

            if (string.IsNullOrEmpty(authorId))
            {
                TempData["Error"] = "Session expired. আবার login করুন।";
                return RedirectToAction("Login", "Auth");
            }

            var dto = new CreateArticleDto
            {
                Title = vm.Title,
                TitleBn = vm.TitleBn,
                Content = vm.Content,
                ContentBn = vm.ContentBn,
                Summary = vm.Summary,
                SummaryBn = vm.SummaryBn,
                ThumbnailUrl = vm.ThumbnailUrl,
                IsBreaking = false,
                IsFeatured = false,
                CategoryId = vm.CategoryId,
                MetaTitle = vm.MetaTitle,
                MetaDescription = vm.MetaDescription,
                MetaKeywords = vm.MetaKeywords
            };

            // ✅ CurrentUserId এর জায়গায় authorId ব্যবহার
            await articleService.CreateAsync(dto, authorId);

            TempData["Success"] = "নিউজ সফলভাবে জমা হয়েছে। Editor অনুমোদন করলে প্রকাশিত হবে।";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            vm.Categories = await categoryService.GetAllWithChildrenAsync();
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var article = await articleService.GetByIdAsync(id);
        if (article is null)
        {
            TempData["Error"] = "নিউজ পাওয়া যায়নি";
            return RedirectToAction(nameof(Index));
        }

        if (CurrentRole == "Reporter" &&
            article.CreatedBy != CurrentUserId)
        {
            TempData["Error"] = "এই নিউজ edit করার অনুমতি নেই";
            return RedirectToAction(nameof(Index));
        }

        if (article.IsPublished && CurrentRole == "Reporter")
        {
            TempData["Error"] = "প্রকাশিত নিউজ edit করা যাবে না";
            return RedirectToAction(nameof(Index));
        }

        var categories = await categoryService.GetAllWithChildrenAsync();
        var vm = new ArticleFormViewModel
        {
            Id = id,
            Title = article.Title,
            TitleBn = article.TitleBn,
            Content = article.Content,
            ContentBn = article.ContentBn,
            Summary = article.Summary,
            SummaryBn = article.SummaryBn,
            ThumbnailUrl = article.ThumbnailUrl,
            IsBreaking = article.IsBreaking,
            IsFeatured = article.IsFeatured,
            IsPublished = article.IsPublished,
            CategoryId = article.CategoryId,
            MetaTitle = article.MetaTitle,
            MetaDescription = article.MetaDescription,
            MetaKeywords = article.MetaKeywords,
            Categories = categories
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ArticleFormViewModel vm)
    {
        try
        {
            var editorId = CurrentUserId;
            var dto = new UpdateArticleDto
            {
                Id = vm.Id,
                Title = vm.Title,
                TitleBn = vm.TitleBn,
                Content = vm.Content,
                ContentBn = vm.ContentBn,
                Summary = vm.Summary,
                SummaryBn = vm.SummaryBn,
                ThumbnailUrl = vm.ThumbnailUrl,
                IsBreaking = CurrentRole != "Reporter" && vm.IsBreaking,
                IsFeatured = CurrentRole != "Reporter" && vm.IsFeatured,
                IsPublished = CurrentRole != "Reporter" && vm.IsPublished,
                CategoryId = vm.CategoryId,
                MetaTitle = vm.MetaTitle,
                MetaDescription = vm.MetaDescription,
                MetaKeywords = vm.MetaKeywords
            };

            await articleService.UpdateAsync(dto, editorId);
            TempData["Success"] = "নিউজ আপডেট হয়েছে";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            vm.Categories = await categoryService.GetAllWithChildrenAsync();
            return View(vm);
        }
    }
}