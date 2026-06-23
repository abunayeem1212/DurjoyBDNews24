using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.Video;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class VideoController(IVideoService videoService)
    : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var result = await videoService
            .GetPagedAsync(page, pageSize);
        return Ok(ApiResponse<PagedResult<VideoNewsDto>>
            .Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await videoService.GetByIdAsync(id);
        if (result is null)
            return NotFound(ApiResponse<string>.Fail(
                "ভিডিও পাওয়া যায়নি"));

        await videoService.IncrementViewAsync(id);
        return Ok(ApiResponse<VideoNewsDto>.Ok(result));
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured(
        [FromQuery] int count = 6)
    {
        var result = await videoService.GetFeaturedAsync(count);
        return Ok(ApiResponse<IEnumerable<VideoNewsDto>>
            .Ok(result));
    }

    [Authorize(Roles = "Reporter,Editor,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVideoNewsDto dto)
    {
        var result = await videoService
            .CreateAsync(dto, CurrentUserId);
        return Ok(ApiResponse<VideoNewsDto>.Ok(result,
            "ভিডিও যোগ হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await videoService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("",
            "ভিডিও মুছে ফেলা হয়েছে"));
    }
}