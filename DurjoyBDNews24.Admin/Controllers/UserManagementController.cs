using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DurjoyBDNews24.Domain.Entities;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class UserManagementController(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        var userRoles = new Dictionary<string, IList<string>>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles;
        }

        ViewBag.UserRoles = userRoles;
        ViewBag.AllRoles = roleManager.Roles
            .Select(r => r.Name).ToList();

        return View(users);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ChangeRole(
        string userId, string newRole)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Json(new { success = false, message = "User পাওয়া যায়নি" });

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, newRole);

        return Json(new
        {
            success = true,
            message = $"Role পরিবর্তন হয়েছে: {newRole}"
        });
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]

    public async Task<IActionResult> ToggleActive(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Json(new { success = false });

        user.IsActive = !user.IsActive;
        await userManager.UpdateAsync(user);

        return Json(new
        {
            success = true,
            isActive = user.IsActive
        });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["Error"] = "User পাওয়া যায়নি";
            return RedirectToAction(nameof(Index));
        }

        await userManager.DeleteAsync(user);
        TempData["Success"] = "User মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }
}