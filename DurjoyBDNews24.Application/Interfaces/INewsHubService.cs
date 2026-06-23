using DurjoyBDNews24.Application.DTOs.Article;

namespace DurjoyBDNews24.Application.Interfaces;

public interface INewsHubService
{
    Task BroadcastBreakingNewsAsync(ArticleDto article);
    Task BroadcastNewArticleAsync(ArticleDto article, string categorySlug);
    Task BroadcastViewCountAsync(int articleId, long count);
}