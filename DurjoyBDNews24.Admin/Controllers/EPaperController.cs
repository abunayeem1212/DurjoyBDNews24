using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Application.DTOs.EPaper;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class EPaperController(IEPaperService ePaperService)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var epapers = await ePaperService.GetPagedAsync(1, 50);
        return View(epapers.Items);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(
        string titleBn, string title, string pdfUrl,
        string? thumbnailUrl, DateTime publishDate,
        bool isPublished, bool isPremiumOnly, string? edition)
    {
        try
        {
            await ePaperService.CreateAsync(new CreateEPaperDto
            {
                TitleBn = titleBn,
                Title = title,
                PdfUrl = pdfUrl,
                ThumbnailUrl = thumbnailUrl,
                PublishDate = publishDate,
                IsPublished = isPublished,
                IsPremiumOnly = isPremiumOnly,
                Edition = edition
            });

            TempData["Success"] = "ই-পেপার যোগ হয়েছে";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await ePaperService.DeleteAsync(id);
        TempData["Success"] = "ই-পেপার মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }
}