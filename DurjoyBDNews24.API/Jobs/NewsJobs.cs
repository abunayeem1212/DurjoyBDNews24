using DurjoyBDNews24.Application.Interfaces;
using Hangfire;

namespace DurjoyBDNews24.API.Jobs;

public class NewsJobs(IViewCountService viewCountService)
{
    public async Task FlushViewCountsAsync()
        => await viewCountService.FlushToDbAsync();
}