using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext ctx) : IUserRepository
{
    public async Task<AppUser?> GetByIdStringAsync(string id)
        => await ctx.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task UpdateUserAsync(AppUser user)
    {
        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
    }
}