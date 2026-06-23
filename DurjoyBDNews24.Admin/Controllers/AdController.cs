using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Enums;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class AdController(ApplicationDbContext ctx) : Controller
{
    public async Task<IActionResult> Index()
    {
        var ads = await ctx.Advertisements
            .Include(a => a.Zone)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return View(ads);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Zones = await ctx.AdZones
            .Where(z => z.IsActive)
            .ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        string title, string imageUrl, string targetUrl,
        string advertiserName, string advertiserEmail,
        decimal dailyRate, int dailyCap, int zoneId,
        DateTime startDate, DateTime endDate)
    {
        var ad = new Advertisement
        {
            Title = title,
            ImageUrl = imageUrl,
            TargetUrl = targetUrl,
            AdvertiserName = advertiserName,
            AdvertiserEmail = advertiserEmail,
            DailyRate = dailyRate,
            DailyCap = dailyCap,
            ZoneId = zoneId,
            StartDate = startDate,
            EndDate = endDate,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };

        ctx.Advertisements.Add(ad);
        await ctx.SaveChangesAsync();

        TempData["Success"] = "বিজ্ঞাপন তৈরি হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var ad = await ctx.Advertisements.FindAsync(id);
        if (ad is null) return Json(new { success = false });

        ad.IsApproved = !ad.IsApproved;
        await ctx.SaveChangesAsync();

        return Json(new
        {
            success = true,
            isActive = ad.IsApproved
        });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var ad = await ctx.Advertisements.FindAsync(id);
        if (ad is null)
        {
            TempData["Error"] = "বিজ্ঞাপন পাওয়া যায়নি";
            return RedirectToAction(nameof(Index));
        }

        ctx.Advertisements.Remove(ad);
        await ctx.SaveChangesAsync();

        TempData["Success"] = "বিজ্ঞাপন মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Zones()
    {
        var zones = await ctx.AdZones.ToListAsync();
        return View(zones);
    }

    [HttpPost]
    public async Task<IActionResult> CreateZone(
        string name, AdPosition position, string size)
    {
        ctx.AdZones.Add(new AdZone
        {
            Name = name,
            Position = position,
            Size = size,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();
        TempData["Success"] = "Ad Zone তৈরি হয়েছে";
        return RedirectToAction(nameof(Zones));
    }
}