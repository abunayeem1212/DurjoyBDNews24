using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DurjoyBDNews24.Application.Services;

public class ViewCountService(
    IUnitOfWork uow,
    ILogger<ViewCountService> logger) : IViewCountService
{
    private static readonly ConcurrentDictionary<int, long> _counts = new();

    public Task IncrementAsync(int articleId)
    {
        _counts.AddOrUpdate(articleId, 1, (_, old) => old + 1);
        return Task.CompletedTask;
    }

    public async Task FlushToDbAsync()
    {
        if (_counts.IsEmpty) return;

        var snapshot = _counts.ToList();
        foreach (var (articleId, _) in snapshot)
            _counts.TryRemove(articleId, out _);

        foreach (var (articleId, count) in snapshot)
        {
            try
            {
                for (int i = 0; i < count; i++)
                    await uow.Articles.IncrementViewCountAsync(articleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "View count flush failed for article {Id}", articleId);
            }
        }

        logger.LogInformation("View counts flushed for {Count} articles", snapshot.Count);
    }
}