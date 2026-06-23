using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DurjoyBDNews24.API.Hubs;

public class NewsHubService(IHubContext<NewsHub, INewsHubClient> hubContext)
    : INewsHubService
{
    public async Task BroadcastBreakingNewsAsync(ArticleDto article)
    {
        await hubContext.Clients.Group("all-readers")
            .ReceiveBreakingNews(article);

        await hubContext.Clients.Group("all-readers")
            .ReceiveBreakingTicker(article.TitleBn, article.Slug);
    }

    public async Task BroadcastNewArticleAsync(ArticleDto article, string categorySlug)
    {
        await hubContext.Clients.Group("all-readers")
            .ReceiveNewArticle(article);

        if (!string.IsNullOrEmpty(categorySlug))
            await hubContext.Clients.Group(categorySlug)
                .ReceiveNewArticle(article);
    }

    public async Task BroadcastViewCountAsync(int articleId, long count)
    {
        await hubContext.Clients.Group("all-readers")
            .ReceiveViewCount(articleId, count);
    }
}