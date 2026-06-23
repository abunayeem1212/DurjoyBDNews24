using DurjoyBDNews24.Application.DTOs.Video;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Admin.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class VideoController(
    IVideoService videoService,
    ILiveTVService liveTVService,
    ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var videos = await videoService.GetPagedAsync(1, 50);
        return View(videos.Items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await categoryService
            .GetAllWithChildrenAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        string titleBn, string title,
        string youtubeUrl, string? descriptionBn,
        bool isFeatured, bool isPublished, int? categoryId)
    {
        try
        {
            var authorId = HttpContext.Session
                .GetString("AdminUserId") ?? "";

            await videoService.CreateAsync(new CreateVideoNewsDto
            {
                TitleBn = titleBn,
                Title = title,
                YoutubeUrl = youtubeUrl,
                DescriptionBn = descriptionBn,
                IsFeatured = isFeatured,
                IsPublished = isPublished,
                CategoryId = categoryId
            }, authorId);

            TempData["Success"] = "ভিডিও যোগ হয়েছে";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            ViewBag.Categories = await categoryService
                .GetAllWithChildrenAsync();
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await videoService.DeleteAsync(id);
        TempData["Success"] = "ভিডিও মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> LiveTV()
    {
        var channels = await liveTVService.GetAllActiveAsync();
        return View(channels);
    }

    [HttpGet]
    public IActionResult AddChannel() => View();

    [HttpPost]
    public async Task<IActionResult> AddChannel(
        string nameBn, string name,
        string? description, string streamUrl,
        string? youtubeStreamId, string? thumbnailUrl,
        int sortOrder)
    {
        await liveTVService.CreateAsync(new CreateLiveTVDto
        {
            NameBn = nameBn,
            Name = name,
            Description = description,
            StreamUrl = streamUrl,
            YoutubeStreamId = youtubeStreamId,
            ThumbnailUrl = thumbnailUrl,
            SortOrder = sortOrder
        });

        TempData["Success"] = "Channel তৈরি হয়েছে";
        return RedirectToAction(nameof(LiveTV));
    }

    [HttpPost]
    public async Task<IActionResult> SetLive(
        int id, bool value)
    {
        await liveTVService.SetLiveAsync(id, value);
        return Json(new
        {
            success = true,
            message = value
                ? "Live শুরু হয়েছে"
                : "Live বন্ধ হয়েছে"
        });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteChannel(int id)
    {
        await liveTVService.DeleteAsync(id);
        TempData["Success"] = "Channel মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(LiveTV));
    }
}