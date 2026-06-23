using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class NewsletterController(
    INewsletterService newsletterService) : BaseController
{
    [HttpPost("subscribe")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Subscribe(
        [FromBody] NewsletterSubscribeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(ApiResponse<string>.Fail(
                "ইমেইল দিন"));

        var result = await newsletterService
            .SubscribeAsync(dto.Email, dto.Name);

        return Ok(ApiResponse<string>.Ok("",
            result
                ? "সাবস্ক্রিপশন সফল হয়েছে!"
                : "এই ইমেইল আগেই সাবস্ক্রাইব করা আছে"));
    }

    [HttpGet("unsubscribe/{token}")]
    public async Task<IActionResult> Unsubscribe(string token)
    {
        await newsletterService.UnsubscribeAsync(token);
        return Ok(ApiResponse<string>.Ok("",
            "আনসাবস্ক্রাইব সফল হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPost("send")]
    public async Task<IActionResult> SendNewsletter(
        [FromBody] SendNewsletterDto dto)
    {
        await newsletterService.SendToAllAsync(
            dto.Subject, dto.HtmlBody);
        return Ok(ApiResponse<string>.Ok("",
            "Newsletter পাঠানো হচ্ছে"));
    }
}

public class NewsletterSubscribeDto
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public class SendNewsletterDto
{
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
}