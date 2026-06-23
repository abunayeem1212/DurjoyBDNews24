using DurjoyBDNews24.Application.DTOs.Comment;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class CommentController(ICommentService commentService) : BaseController
{
    [HttpGet("article/{articleId}")]
    public async Task<IActionResult> GetByArticle(int articleId)
    {
        var result = await commentService.GetByArticleAsync(articleId);
        return Ok(ApiResponse<IEnumerable<CommentDto>>.Ok(result));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        var result = await commentService.CreateAsync(dto, CurrentUserId);
        return Ok(ApiResponse<CommentDto>.Ok(result,
            "মন্তব্য পাঠানো হয়েছে। অনুমোদনের পর দেখা যাবে।"));
    }


    [HttpPost("guest")]
    public async Task<IActionResult> CreateGuest([FromBody] GuestCommentDto dto)
    {
        var result = await commentService.CreateGuestAsync(dto);
        return Ok(ApiResponse<CommentDto>.Ok(result,
            "মন্তব্য পাঠানো হয়েছে। অনুমোদনের পর দেখা যাবে।"));
    }


    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        await commentService.ApproveAsync(id);
        return Ok(ApiResponse<string>.Ok("", "মন্তব্য অনুমোদিত হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await commentService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("", "মন্তব্য মুছে ফেলা হয়েছে"));
    }



}