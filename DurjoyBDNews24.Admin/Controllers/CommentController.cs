using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Application.DTOs.Comment;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class CommentController(
    ICommentService commentService,
    IUnitOfWork uow) : Controller
{
    public async Task<IActionResult> Index()
    {
        var pending = await commentService.GetPendingAsync();
        var autoApprove = await uow.Settings
            .GetValueAsync("comment_auto_approve");
        ViewBag.AutoApprove = autoApprove == "true";
        return View(pending);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        await commentService.ApproveAsync(id);
        TempData["Success"] = "মন্তব্য অনুমোদিত হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await commentService.DeleteAsync(id);
        TempData["Success"] = "মন্তব্য মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAll()
    {
        var pending = await commentService.GetPendingAsync();
        foreach (var comment in pending)
            await commentService.ApproveAsync(comment.Id);
        TempData["Success"] = $"{pending.Count()} টি মন্তব্য অনুমোদিত হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ToggleAutoApprove()
    {
        var current = await uow.Settings
            .GetValueAsync("comment_auto_approve");
        var newValue = current == "true" ? "false" : "true";
        await uow.Settings
            .SetValueAsync("comment_auto_approve", newValue);
        return Json(new
        {
            success = true,
            autoApprove = newValue == "true"
        });
    }

    [HttpPost("guest")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CreateGuest([FromBody] GuestCommentDto dto)
    {
        var result = await commentService.CreateGuestAsync(dto);
        return Ok(ApiResponse<CommentDto>.Ok(result,
            "মন্তব্য পাঠানো হয়েছে। অনুমোদনের পর দেখা যাবে।"));
    }

}