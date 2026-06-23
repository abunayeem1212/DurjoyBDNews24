using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext ctx) : IUnitOfWork
{
    private readonly ApplicationDbContext _ctx = ctx;
    private IDbContextTransaction? _transaction;

    public IArticleRepository Articles { get; } =
        new ArticleRepository(ctx);
    public ICategoryRepository Categories { get; } =
        new CategoryRepository(ctx);
    public ISubscriptionRepository Subscriptions { get; } =
        new SubscriptionRepository(ctx);
    public IUserRepository Users { get; } =
        new UserRepository(ctx);
    public ICommentRepository Comments { get; } =
        new CommentRepository(ctx);
    public ISettingRepository Settings { get; } =
        new SettingRepository(ctx);
    public INewsletterRepository Newsletter { get; } =
        new NewsletterRepository(ctx);
    public IVideoRepository Videos { get; } =
        new VideoRepository(ctx);
    public ILiveTVRepository LiveTV { get; } =
        new LiveTVRepository(ctx);
    public IEPaperRepository EPapers { get; } =
        new EPaperRepository(ctx);

    public async Task<int> SaveChangesAsync() =>
        await _ctx.SaveChangesAsync();

    public async Task BeginTransactionAsync() =>
        _transaction = await _ctx.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        await _transaction!.CommitAsync();
        await _transaction.DisposeAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction!.RollbackAsync();
        await _transaction.DisposeAsync();
    }

    public void Dispose() => _ctx.Dispose();
}