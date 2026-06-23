using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Admin.Models;
using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class NewsController(
    IArticleService articleService,
    ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index(
     [FromQuery] int page = 1,
     [FromQuery] string? search = null)
    {
        var result = await articleService.GetAllForAdminAsync(page, 20, search);
        var vm = new ArticleListViewModel
        {
            Articles = result.Items,
            CurrentPage = page,
            TotalPages = result.TotalPages,
            TotalCount = result.TotalCount,
            SearchTerm = search
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await categoryService.GetAllWithChildrenAsync();
        return View(new ArticleFormViewModel { Categories = categories });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ArticleFormViewModel vm)
    {
        try
        {
            var authorId = HttpContext.Session.GetString("AdminUserId") ?? "";
            var dto = new CreateArticleDto
            {
                Title = vm.Title,
                TitleBn = vm.TitleBn,
                Content = vm.Content,
                ContentBn = vm.ContentBn,
                Summary = vm.Summary,
                SummaryBn = vm.SummaryBn,
                ThumbnailUrl = vm.ThumbnailUrl,
                IsBreaking = vm.IsBreaking,
                IsFeatured = vm.IsFeatured,
                CategoryId = vm.CategoryId,
                MetaTitle = vm.MetaTitle,
                MetaDescription = vm.MetaDescription,
                MetaKeywords = vm.MetaKeywords
            };

            var result = await articleService.CreateAsync(dto, authorId);

            if (vm.IsPublished)
                await articleService.PublishAsync(result.Id);

            TempData["Success"] = "নিউজ সফলভাবে তৈরি হয়েছে";
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
            var editorId = HttpContext.Session.GetString("AdminUserId") ?? "";
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
                IsBreaking = vm.IsBreaking,
                IsFeatured = vm.IsFeatured,
                IsPublished = vm.IsPublished,
                CategoryId = vm.CategoryId,
                MetaTitle = vm.MetaTitle,
                MetaDescription = vm.MetaDescription,
                MetaKeywords = vm.MetaKeywords
            };

            await articleService.UpdateAsync(dto, editorId);
            TempData["Success"] = "নিউজ সফলভাবে আপডেট হয়েছে";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            vm.Categories = await categoryService.GetAllWithChildrenAsync();
            return View(vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Publish(int id)
    {
        await articleService.PublishAsync(id);
        TempData["Success"] = "নিউজ প্রকাশিত হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await articleService.DeleteAsync(id);
        TempData["Success"] = "নিউজ মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SetBreaking(int id, bool value)
    {
        await articleService.SetBreakingAsync(id, value);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> SetFeatured(int id, bool value)
    {
        await articleService.SetFeaturedAsync(id, value);
        return Json(new { success = true });
    }


    public async Task<IActionResult> PendingApproval()
    {
        var result = await articleService.GetAllForAdminAsync(1, 100);
        var pending = result.Items
            .Where(a => !a.IsPublished)
            .ToList();

        return View(pending);
    }

}