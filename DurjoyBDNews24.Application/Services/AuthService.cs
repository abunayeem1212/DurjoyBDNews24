using DurjoyBDNews24.Application.DTOs.Auth;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Enums;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DurjoyBDNews24.Application.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IConfiguration config) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("ইমেইল বা পাসওয়ার্ড সঠিক নয়");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("আপনার একাউন্ট নিষ্ক্রিয় করা হয়েছে");

        var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            await userManager.AccessFailedAsync(user);
            if (await userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("অনেকবার ভুল পাসওয়ার্ড দেওয়ায় একাউন্ট সাময়িক বন্ধ আছে");
            throw new UnauthorizedAccessException("ইমেইল বা পাসওয়ার্ড সঠিক নয়");
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            int.Parse(config["Jwt:RefreshTokenExpiry"]!));
        await userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(config["Jwt:AccessTokenExpiry"]!)),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Reader"
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await userManager.FindByEmailAsync(dto.Email);
        if (exists is not null)
            throw new InvalidOperationException("এই ইমেইল দিয়ে আগেই একাউন্ট আছে");

        var user = new AppUser
        {
            FullName = dto.FullName,
            FullNameBn = dto.FullNameBn,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.Phone,
            Role = UserRole.Reader,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new InvalidOperationException(string.Join(", ", errors));
        }

        await userManager.AddToRoleAsync(user, "Reader");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            int.Parse(config["Jwt:RefreshTokenExpiry"]!));
        await userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(config["Jwt:AccessTokenExpiry"]!)),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = "Reader"
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var user = userManager.Users
            .FirstOrDefault(u => u.RefreshToken == dto.RefreshToken)
            ?? throw new UnauthorizedAccessException("Refresh token সঠিক নয়");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token মেয়াদ শেষ, আবার login করুন");

        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            int.Parse(config["Jwt:RefreshTokenExpiry"]!));
        await userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(config["Jwt:AccessTokenExpiry"]!)),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Reader"
        };
    }

    public async Task LogoutAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await userManager.UpdateAsync(user);
    }
}