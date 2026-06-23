using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class SubscriptionRepository(ApplicationDbContext ctx)
    : BaseRepository<Subscription>(ctx), ISubscriptionRepository
{
    public async Task<Subscription?> FindFirstAsync(
        Expression<Func<Subscription, bool>> predicate)
        => await _ctx.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
}