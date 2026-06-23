using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.Video;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class LiveTVController(ILiveTVService liveTVService)
    : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await liveTVService.GetAllActiveAsync();
        return Ok(ApiResponse<IEnumerable<LiveTVDto>>
            .Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await liveTVService.GetByIdAsync(id);
        if (result is null)
            return NotFound(
                ApiResponse<string>.Fail("Channel পাওয়া যায়নি"));
        return Ok(ApiResponse<LiveTVDto>.Ok(result));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLiveTVDto dto)
    {
        var result = await liveTVService.CreateAsync(dto);
        return Ok(ApiResponse<LiveTVDto>.Ok(result,
            "Channel তৈরি হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] CreateLiveTVDto dto)
    {
        await liveTVService.UpdateAsync(id, dto);
        return Ok(ApiResponse<string>.Ok("",
            "Channel আপডেট হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await liveTVService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("",
            "Channel মুছে ফেলা হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPatch("{id}/live")]
    public async Task<IActionResult> SetLive(
        int id, [FromQuery] bool value)
    {
        await liveTVService.SetLiveAsync(id, value);
        return Ok(ApiResponse<string>.Ok("",
            value ? "Live শুরু হয়েছে" : "Live বন্ধ হয়েছে"));
    }
}