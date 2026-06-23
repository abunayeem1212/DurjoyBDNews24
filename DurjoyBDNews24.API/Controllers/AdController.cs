using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Domain.Enums;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.API.Controllers;

public class AdController(ApplicationDbContext ctx) : BaseController
{
    [HttpGet("zone/{position}")]
    public async Task<IActionResult> GetByZone(int position)
    {
        var now = DateTime.UtcNow;
        var ad = await ctx.Advertisements
            .Include(a => a.Zone)
            .Where(a => a.IsApproved
                     && !a.IsDeleted
                     && a.Zone.Position == (AdPosition)position
                     && a.StartDate <= now
                     && a.EndDate >= now
                     && a.ImpressionCount < a.DailyCap)
            .OrderBy(_ => Guid.NewGuid())
            .FirstOrDefaultAsync();

        if (ad is null) return Ok(ApiResponse<object>.Ok(null!));

        ad.ImpressionCount++;
        await ctx.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new
        {
            id = ad.Id,
            imageUrl = ad.ImageUrl,
            targetUrl = $"/api/v1/ad/click/{ad.Id}",
            title = ad.Title,
            size = ad.Zone.Size
        }));
    }

    [HttpGet("click/{id}")]
    public async Task<IActionResult> Click(int id,
        [FromQuery] string returnUrl = "/")
    {
        var ad = await ctx.Advertisements.FindAsync(id);
        if (ad is not null)
        {
            ad.ClickCount++;
            await ctx.SaveChangesAsync();
            return Redirect(ad.TargetUrl);
        }
        return Redirect(returnUrl);
    }
}