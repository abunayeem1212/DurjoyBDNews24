using DurjoyBDNews24.API.Filters;
using DurjoyBDNews24.API.Hubs;
using DurjoyBDNews24.API.Jobs;
using DurjoyBDNews24.API.Middleware;
using DurjoyBDNews24.Application.Extensions;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using DurjoyBDNews24.Infrastructure.Extensions;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/durjoybdnews-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("DurjoyPolicy", policy =>
        {
            policy.WithOrigins(
                    // Local
                    "https://localhost:7002",
                    "https://localhost:7003",
                    "http://localhost:5002",
                    "http://localhost:5003",
                    "http://localhost:8001",
                    "http://localhost:8002",
                    "http://localhost:8003",

                    // Live
                    "https://durjoybdnews24.com",
                    "https://www.durjoybdnews24.com",
                    "https://admin.durjoybdnews24.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddRateLimiter(opts =>
    {
        opts.RejectionStatusCode = 429;
        opts.AddFixedWindowLimiter("public", o =>
        {
            o.PermitLimit = 60;
            o.Window = TimeSpan.FromMinutes(1);
        });
        opts.AddFixedWindowLimiter("auth", o =>
        {
            o.PermitLimit = 5;
            o.Window = TimeSpan.FromMinutes(1);
        });
        opts.OnRejected = async (ctx, token) =>
        {
            ctx.HttpContext.Response.Headers.RetryAfter = "60";
            await ctx.HttpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "অনেক বেশি request পাঠিয়েছেন। ১ মিনিট অপেক্ষা করুন।"
            }, token);
        };
    });

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddSignalR();
    builder.Services.AddMemoryCache();

    builder.Services.AddResponseCaching();
    builder.Services.AddOutputCache(opt =>
    {
        opt.AddPolicy("home", b =>
            b.Expire(TimeSpan.FromMinutes(5)));
        opt.AddPolicy("article", b =>
            b.Expire(TimeSpan.FromMinutes(30)));
        opt.AddPolicy("category", b =>
            b.Expire(TimeSpan.FromMinutes(10)));
    });




    builder.Services.AddScoped<INewsHubService, NewsHubService>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        await DbSeeder.SeedAsync(scope.ServiceProvider);

        // Initialize Hangfire recurring jobs using service-based API
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.AddOrUpdate<NewsJobs>(
            "flush-view-counts",
            job => job.FlushViewCountsAsync(),
            "*/5 * * * *");
    }

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.Use(async (ctx, next) =>
    {
        ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        ctx.Response.Headers.Append("X-Frame-Options", "DENY");
        ctx.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        ctx.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        await next();
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DurjoyBDNews24 API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseSerilogRequestLogging();
    app.UseCors("DurjoyPolicy");
    app.UseRateLimiter();
    app.UseHttpsRedirection();

    app.UseResponseCaching();
    app.UseOutputCache();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseStaticFiles();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        DashboardTitle = "DurjoyBDNews24 — Background Jobs",
        Authorization = [new HangfireAuthFilter()]
    });

    app.MapHangfireDashboard();
    app.MapHub<NewsHub>("/hubs/news");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DurjoyBDNews24 API চালু হতে পারেনি");
}
finally
{
    Log.CloseAndFlush();
}