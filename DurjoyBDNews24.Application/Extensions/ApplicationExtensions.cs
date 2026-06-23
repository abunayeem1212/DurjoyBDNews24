using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Application.Mappings;
using DurjoyBDNews24.Application.Services;
using DurjoyBDNews24.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DurjoyBDNews24.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ArticleMappingProfile));

        services.AddValidatorsFromAssemblyContaining<CreateArticleValidator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISlugService, SlugService>();
        services.AddScoped<ICacheService, CacheService>();

        services.AddScoped<INewsletterService, NewsletterService>();

        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ILiveTVService, LiveTVService>();

        services.AddScoped<IPaymentService, PaymentService>();
        services.AddHttpClient<IPaymentService, PaymentService>();

        services.AddScoped<IEPaperService, EPaperService>();

        services.AddScoped<IViewCountService, ViewCountService>();

        services.AddScoped<ICommentService, CommentService>();

        return services;
    }
}