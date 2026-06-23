using Hangfire.Dashboard;

namespace DurjoyBDNews24.API.Filters;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        return http.User.Identity?.IsAuthenticated == true
            && http.User.IsInRole("SuperAdmin");
    }
}