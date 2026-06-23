using AutoMapper;
using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Category;
using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Application.Mappings;

public class ArticleMappingProfile : Profile
{
    public ArticleMappingProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category != null ? s.Category.Name : ""))
            .ForMember(d => d.CategoryNameBn,
                o => o.MapFrom(s => s.Category != null ? s.Category.NameBn : ""))
            .ForMember(d => d.CategorySlug,
                o => o.MapFrom(s => s.Category != null ? s.Category.Slug : ""))
            .ForMember(d => d.AuthorName,
                o => o.MapFrom(s => s.Author != null ? s.Author.FullName : ""))
            .ForMember(d => d.AuthorNameBn,
                o => o.MapFrom(s => s.Author != null ? s.Author.FullNameBn : ""))
            .ForMember(d => d.IsPublished,
                o => o.MapFrom(s => s.IsPublished))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Tags,
                o => o.MapFrom(s => s.ArticleTags != null
                    ? s.ArticleTags.Select(at => at.Tag.NameBn).ToList()
                    : new List<string>()))

            .ForMember(d => d.CategoryId,
                o => o.MapFrom(s => s.CategoryId))
            .ForMember(d => d.Content,
                o => o.MapFrom(s => s.Content))
            .ForMember(d => d.ContentBn,
                o => o.MapFrom(s => s.ContentBn))
            .ForMember(d => d.MetaTitle,
                o => o.MapFrom(s => s.MetaTitle))
            .ForMember(d => d.MetaDescription,
                o => o.MapFrom(s => s.MetaDescription))
            .ForMember(d => d.MetaKeywords,
                o => o.MapFrom(s => s.MetaKeywords))

            .ForMember(d => d.CreatedBy,
                o => o.MapFrom(s => s.CreatedBy));


        CreateMap<Article, ArticleDetailDto>()
            .IncludeBase<Article, ArticleDto>()
            .ForMember(d => d.RelatedArticles,
                o => o.Ignore());

        CreateMap<CreateArticleDto, Article>()
            .ForMember(d => d.ArticleTags, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore());

        CreateMap<UpdateArticleDto, Article>()
            .ForMember(d => d.ArticleTags, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore());

        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.Children,
                o => o.MapFrom(s => s.Children != null
                    ? s.Children.Where(c => c.IsActive).ToList()
                    : new List<Category>()));

        CreateMap<CreateCategoryDto, Category>()
            .ForMember(d => d.Children, o => o.Ignore())
            .ForMember(d => d.Parent, o => o.Ignore())
            .ForMember(d => d.Articles, o => o.Ignore());
    }
}