using AutoMapper;
using DurjoyBDNews24.Application.DTOs.Category;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace DurjoyBDNews24.Application.Services;

public class CategoryService(
    IUnitOfWork uow,
    IMapper mapper) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllWithChildrenAsync()
    {
        var categories = await uow.Categories.GetActiveWithChildrenAsync();
        return mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug)
    {
        var category = await uow.Categories.GetBySlugAsync(slug);
        return category is null ? null : mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var slug = GenerateSlug(dto.Name, dto.NameBn);

        var exists = await uow.Categories.ExistsAsync(c => c.Slug == slug);
        if (exists)
            slug = $"{slug}-{DateTime.UtcNow.Ticks}";

        var category = mapper.Map<Category>(dto);
        category.Slug = slug;
        category.IsActive = true;
        category.CreatedAt = DateTime.UtcNow;

        await uow.Categories.AddAsync(category);
        await uow.SaveChangesAsync();
        return mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await uow.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"বিভাগ পাওয়া যায়নি: {id}");

        mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;

        await uow.Categories.UpdateAsync(category);
        await uow.SaveChangesAsync();
        return mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await uow.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"বিভাগ পাওয়া যায়নি: {id}");
        category.IsDeleted = true;
        await uow.Categories.UpdateAsync(category);
        await uow.SaveChangesAsync();
    }

    private static string GenerateSlug(string name, string nameBn)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var slug = name.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = Regex.Replace(slug, @"-+", "-").Trim('-');
            if (!string.IsNullOrWhiteSpace(slug)) return slug;
        }

        var bnSlug = nameBn.Trim();
        bnSlug = Regex.Replace(bnSlug, @"\s+", "-");
        bnSlug = Regex.Replace(bnSlug, @"[^\u0980-\u09FF\-]", "");
        return Regex.Replace(bnSlug, @"-+", "-").Trim('-');
    }
}