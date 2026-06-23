using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.DTOs.EPaper;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class EPaperController(IEPaperService ePaperService)
    : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var result = await ePaperService
            .GetPagedAsync(page, pageSize);
        return Ok(ApiResponse<PagedResult<EPaperDto>>
            .Ok(result));
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var result = await ePaperService.GetTodayAsync();
        return Ok(ApiResponse<EPaperDto?>.Ok(result));
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent(
        [FromQuery] int count = 7)
    {
        var result = await ePaperService.GetRecentAsync(count);
        return Ok(ApiResponse<IEnumerable<EPaperDto>>
            .Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await ePaperService.GetByIdAsync(id);
        if (result is null)
            return NotFound(ApiResponse<string>.Fail(
                "E-Paper পাওয়া যায়নি"));
        await ePaperService.IncrementViewAsync(id);
        return Ok(ApiResponse<EPaperDto>.Ok(result));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var epaper = await ePaperService.GetByIdAsync(id);
        if (epaper is null)
            return NotFound(ApiResponse<string>.Fail(
                "E-Paper পাওয়া যায়নি"));

        await ePaperService.IncrementDownloadAsync(id);
        return Ok(ApiResponse<string>.Ok(
            epaper.PdfUrl, "Download শুরু করুন"));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEPaperDto dto)
    {
        var result = await ePaperService.CreateAsync(dto);
        return Ok(ApiResponse<EPaperDto>.Ok(result,
            "E-Paper যোগ হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await ePaperService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("",
            "E-Paper মুছে ফেলা হয়েছে"));
    }
}