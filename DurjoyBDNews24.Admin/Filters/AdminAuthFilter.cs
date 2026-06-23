using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DurjoyBDNews24.Admin.Filters;

public class AdminAuthFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        var token = ctx.HttpContext.Session.GetString("AdminToken");
        var role = ctx.HttpContext.Session.GetString("AdminRole");

        if (string.IsNullOrEmpty(token))
        {
            ctx.Result = new RedirectToActionResult(
                "Login", "Auth", null);
            return;
        }

        var allowedRoles = new[]
        {
            "SuperAdmin", "Editor", "Reporter"
        };

        if (!allowedRoles.Contains(role))
        {
            ctx.Result = new RedirectToActionResult(
                "Login", "Auth", null);
            return;
        }

        base.OnActionExecuting(ctx);
    }
}