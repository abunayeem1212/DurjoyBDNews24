namespace DurjoyBDNews24.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IArticleRepository Articles { get; }
    ICategoryRepository Categories { get; }
    ISubscriptionRepository Subscriptions { get; }
    IUserRepository Users { get; }
    ICommentRepository Comments { get; }
    ISettingRepository Settings { get; }
    INewsletterRepository Newsletter { get; }
    IVideoRepository Videos { get; }
    ILiveTVRepository LiveTV { get; }
    IEPaperRepository EPapers { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}