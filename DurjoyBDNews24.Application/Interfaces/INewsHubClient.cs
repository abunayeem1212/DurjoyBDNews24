using DurjoyBDNews24.Application.DTOs.Article;

namespace DurjoyBDNews24.Application.Interfaces;

public interface INewsHubClient
{
    Task ReceiveBreakingNews(ArticleDto article);
    Task ReceiveNewArticle(ArticleDto article);
    Task ReceiveViewCount(int articleId, long count);
    Task ReceiveBreakingTicker(string title, string slug);
}