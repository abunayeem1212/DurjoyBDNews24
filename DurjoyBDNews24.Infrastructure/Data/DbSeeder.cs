using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DurjoyBDNews24.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var ctx = services.GetRequiredService<ApplicationDbContext>();

        await ctx.Database.MigrateAsync();

        string[] roles = ["SuperAdmin", "Editor", "Reporter", "Reader"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByEmailAsync("admin@durjoybdnews24.com") is null)
        {
            var admin = new AppUser
            {
                FullName = "Super Admin",
                FullNameBn = "সুপার অ্যাডমিন",
                Email = "admin@durjoybdnews24.com",
                UserName = "admin@durjoybdnews24.com",
                Role = UserRole.SuperAdmin,
                IsActive = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
        }

        if (!await ctx.Categories.AnyAsync())
        {
            var categories = new List<Domain.Entities.Category>
            {
                new() { Name = "National", NameBn = "জাতীয়", Slug = "national", SortOrder = 1, IsActive = true },
                new() { Name = "International", NameBn = "আন্তর্জাতিক", Slug = "international", SortOrder = 2, IsActive = true },
                new() { Name = "Politics", NameBn = "রাজনীতি", Slug = "politics", SortOrder = 3, IsActive = true },
                new() { Name = "Sports", NameBn = "খেলাধুলা", Slug = "sports", SortOrder = 4, IsActive = true },
                new() { Name = "Entertainment", NameBn = "বিনোদন", Slug = "entertainment", SortOrder = 5, IsActive = true },
                new() { Name = "Technology", NameBn = "প্রযুক্তি", Slug = "technology", SortOrder = 6, IsActive = true },
                new() { Name = "Business", NameBn = "ব্যবসা-বাণিজ্য", Slug = "business", SortOrder = 7, IsActive = true },
                new() { Name = "Religion", NameBn = "ধর্ম", Slug = "religion", SortOrder = 8, IsActive = true },
            };
            await ctx.Categories.AddRangeAsync(categories);
            await ctx.SaveChangesAsync();
        }
    }
}