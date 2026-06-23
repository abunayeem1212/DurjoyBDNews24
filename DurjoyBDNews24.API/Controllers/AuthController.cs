using DurjoyBDNews24.Application.DTOs.Auth;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "লগইন সফল হয়েছে"));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "রেজিস্ট্রেশন সফল হয়েছে"));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await authService.RefreshTokenAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Token refresh সফল হয়েছে"));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(CurrentUserId);
        return Ok(ApiResponse<string>.Ok("", "লগআউট সফল হয়েছে"));
    }
}