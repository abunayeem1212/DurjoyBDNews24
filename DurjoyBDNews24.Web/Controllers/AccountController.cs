using DurjoyBDNews24.Web.Models;
using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Web.Controllers;

public class AccountController(IApiService api) : BaseController(api)
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(
        string email, string password, string? returnUrl = null)
    {
        try
        {
            var result = await api.LoginAsync(email, password);

            HttpContext.Session.SetString("UserToken", result.AccessToken);
            HttpContext.Session.SetString("UserRefreshToken", result.RefreshToken);
            HttpContext.Session.SetString("UserName", result.FullName);
            HttpContext.Session.SetString("UserNameBn", result.FullName);
            HttpContext.Session.SetString("UserRole", result.Role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(
        string fullNameBn, string email,
        string password, string phone)
    {
        try
        {
            var result = await api.RegisterAsync(
                fullNameBn, email, password, phone);

            HttpContext.Session.SetString("UserToken", result.AccessToken);
            HttpContext.Session.SetString("UserName", result.FullName);
            HttpContext.Session.SetString("UserNameBn", result.FullName);
            HttpContext.Session.SetString("UserRole", result.Role);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
    }


    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login");

        try
        {
            var subscription = await api.GetMySubscriptionAsync(token);
            var comments = await api.GetMyCommentsAsync(token);

            var vm = new ProfileViewModel
            {
                FullNameBn = HttpContext.Session
                    .GetString("UserNameBn") ?? "",
                Email = HttpContext.Session
                    .GetString("UserEmail") ?? "",
                Role = HttpContext.Session
                    .GetString("UserRole") ?? "Reader",
                Subscription = subscription,
                RecentComments = comments.Take(5)
            };

            return View(vm);
        }
        catch
        {
            return RedirectToAction("Login");
        }
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}