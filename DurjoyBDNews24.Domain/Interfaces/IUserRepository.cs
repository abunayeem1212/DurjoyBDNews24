using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdStringAsync(string id);
    Task UpdateUserAsync(AppUser user);
}