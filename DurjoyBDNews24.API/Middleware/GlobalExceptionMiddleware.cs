using DurjoyBDNews24.Application.DTOs.Common;
using System.Diagnostics;
using System.Net;

namespace DurjoyBDNews24.API.Middleware;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on {Path}", ctx.Request.Path);
            await HandleAsync(ctx, ex);
        }
    }

    private static async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, message) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "সার্ভারে সমস্যা হয়েছে")
        };

        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json";

        await ctx.Response.WriteAsJsonAsync(ApiResponse<string>.Fail(
            message,
            new List<string> { Activity.Current?.Id ?? ctx.TraceIdentifier }));
    }
}