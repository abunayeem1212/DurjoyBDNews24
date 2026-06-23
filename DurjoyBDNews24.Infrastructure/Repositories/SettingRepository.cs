using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class SettingRepository(ApplicationDbContext ctx) : ISettingRepository
{
    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await ctx.SiteSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        var setting = await ctx.SiteSettings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting is null)
        {
            ctx.SiteSettings.Add(new SiteSetting
            {
                Key = key,
                Value = value,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await ctx.SaveChangesAsync();
    }
}