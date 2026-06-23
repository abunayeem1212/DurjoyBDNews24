using DurjoyBDNews24.Admin.Models;
using DurjoyBDNews24.Application.DTOs.Auth;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        var token = HttpContext.Session.GetString("AdminToken");
        if (!string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Dashboard");
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        try
        {
            var result = await authService.LoginAsync(new LoginDto
            {
                Email = vm.Email,
                Password = vm.Password
            });

            if (result.Role is not ("SuperAdmin" or "Editor" or "Reporter"))
            {
                vm.ErrorMessage = "এই একাউন্টে admin access নেই";
                return View(vm);
            }

            HttpContext.Session.SetString("AdminToken", result.AccessToken);
            HttpContext.Session.SetString("AdminRefreshToken", result.RefreshToken);
            HttpContext.Session.SetString("AdminName", result.FullName);
            HttpContext.Session.SetString("AdminRole", result.Role);
            HttpContext.Session.SetString("AdminUserId", result.UserId); // ← এটা add করো


            return RedirectToAction("Index", "Dashboard");
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = ex.Message;
            return View(vm);
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}