using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class MediaController(IMediaService mediaService) : BaseController
{
    [Authorize(Roles = "Reporter,Editor,SuperAdmin")]
    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromQuery] string folder = "articles")
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("ফাইল নির্বাচন করুন"));

        if (!mediaService.IsValidImage(file.FileName, file.Length))
            return BadRequest(ApiResponse<string>.Fail(
                "শুধু JPG, PNG, WEBP ফাইল দেওয়া যাবে। সর্বোচ্চ 10 MB।"));

        await using var stream = file.OpenReadStream();
        var url = await mediaService.UploadImageAsync(
            stream, file.FileName, folder);

        var fullUrl = $"{Request.Scheme}://{Request.Host}{url}";
        return Ok(ApiResponse<string>.Ok(fullUrl, "ছবি upload হয়েছে"));
    }
}