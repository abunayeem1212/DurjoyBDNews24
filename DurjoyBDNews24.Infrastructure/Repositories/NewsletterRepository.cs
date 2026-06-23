using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class NewsletterRepository(ApplicationDbContext ctx)
    : BaseRepository<NewsletterSubscriber>(ctx),
      INewsletterRepository
{
    public async Task<NewsletterSubscriber?> GetByEmailAsync(
        string email) =>
        await _ctx.NewsletterSubscribers
            .FirstOrDefaultAsync(s => s.Email == email);

    public async Task<NewsletterSubscriber?> GetByTokenAsync(
        string token) =>
        await _ctx.NewsletterSubscribers
            .FirstOrDefaultAsync(s =>
                s.UnsubscribeToken == token);

    public async Task<IEnumerable<NewsletterSubscriber>>
        GetActiveAsync() =>
        await _ctx.NewsletterSubscribers
            .AsNoTracking()
            .Where(s => s.IsActive)
            .ToListAsync();
}