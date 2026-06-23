using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace DurjoyBDNews24.Application.Services
{
    public class SlugService(IUnitOfWork uow) : ISlugService
    {
        public string Generate(string title)
        {
            var slug = title.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^\u0980-\u09FFa-z0-9\-]", ""); 
            slug = Regex.Replace(slug, @"-+", "-").Trim('-');
            return slug;
        }

        public async Task<string> GenerateUniqueAsync(string title)
        {
            var baseSlug = Generate(title);
            var slug = baseSlug;
            var counter = 1;

            while (await uow.Articles.ExistsAsync(a => a.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            return slug;
        }
    }
}
