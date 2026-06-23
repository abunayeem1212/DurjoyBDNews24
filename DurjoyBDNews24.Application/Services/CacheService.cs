using DurjoyBDNews24.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DurjoyBDNews24.Application.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await cache.GetStringAsync(key);
        return data is null ? default : JsonSerializer.Deserialize<T>(data, _opts);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var opts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        };
        await cache.SetStringAsync(key, JsonSerializer.Serialize(value, _opts), opts);
    }

    public async Task RemoveAsync(string key) =>
        await cache.RemoveAsync(key);

    public Task RemoveByPatternAsync(string pattern)
    {
        return Task.CompletedTask;
    }
}