using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DurjoyBDNews24.Web.Controllers;

public class EPaperController(
    IApiService api,
    IConfiguration config) : BaseController(api, config)
{
    public async Task<IActionResult> Index(
        [FromQuery] int page = 1)
    {
        var epapers = await api.GetEPapersAsync(page, 12);
        var today = await api.GetTodayEPaperAsync();

        ViewBag.EPapers = epapers;
        ViewBag.TodayEPaper = today;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = epapers.TotalPages;

        return View();
    }

    public async Task<IActionResult> Read(int id)
    {
        var epapers = await api.GetRecentEPapersAsync();
        var epaper = epapers.FirstOrDefault(e => e.Id == id)
            ?? await api.GetTodayEPaperAsync();

        if (epaper is null)
            return RedirectToAction(nameof(Index));

        ViewBag.EPaper = epaper;
        ViewBag.RecentEPapers = epapers.Take(7);

        return View();
    }
}