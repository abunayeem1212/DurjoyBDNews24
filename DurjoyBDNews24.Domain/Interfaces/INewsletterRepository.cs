using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface INewsletterRepository
    : IBaseRepository<NewsletterSubscriber>
{
    Task<NewsletterSubscriber?> GetByEmailAsync(string email);
    Task<NewsletterSubscriber?> GetByTokenAsync(string token);
    Task<IEnumerable<NewsletterSubscriber>> GetActiveAsync();
}