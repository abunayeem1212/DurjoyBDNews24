using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> FindFirstAsync(
        System.Linq.Expressions.Expression<Func<Subscription, bool>> predicate);
}