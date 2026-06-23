using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Domain.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
}