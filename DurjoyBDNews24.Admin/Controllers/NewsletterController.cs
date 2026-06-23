using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class NewsletterController(
    IUnitOfWork uow,
    INewsletterService newsletterService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var subscribers = await uow.Newsletter.GetActiveAsync();
        ViewBag.TotalCount = subscribers.Count();
        return View(subscribers);
    }

    [HttpGet]
    public IActionResult Send() => View();

    [HttpPost]
    public async Task<IActionResult> Send(
        string subject, string body)
    {
        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
                <div style="background:#c0392b;padding:20px;text-align:center;">
                    <h1 style="color:white;font-size:20px;margin:0;">
                        দুর্জয় বিডি নিউজ ২৪
                    </h1>
                </div>
                <div style="padding:24px;background:white;">
                    <h2 style="font-size:18px;color:#1a1a1a;">
                        {subject}
                    </h2>
                    <div style="font-size:15px;color:#555;line-height:1.7;">
                        {body.Replace("\n", "<br/>")}
                    </div>
                </div>
                <div style="background:#f5f5f5;padding:16px;text-align:center;font-size:12px;color:#888;">
                    দুর্জয় বিডি নিউজ ২৪ | ঢাকা, বাংলাদেশ
                </div>
            </div>
        """;

        await newsletterService.SendToAllAsync(subject, html);
        TempData["Success"] = "Newsletter পাঠানো শুরু হয়েছে";
        return RedirectToAction(nameof(Index));
    }
}