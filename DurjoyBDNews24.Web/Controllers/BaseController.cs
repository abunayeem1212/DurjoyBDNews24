using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace DurjoyBDNews24.Web.Controllers;

public class BaseController(
    IApiService api,
    IConfiguration? config = null) : Controller
{
    public override async void OnActionExecuting(
        ActionExecutingContext ctx)
    {
        try
        {
            var breaking = await api.GetBreakingNewsAsync();
            var categories = await api.GetCategoriesAsync();
            ViewBag.BreakingNews = breaking;
            ViewBag.Categories = categories;

            if (config != null)
            {
                ViewBag.GoogleAnalyticsId =
                    config["Analytics:GoogleAnalyticsId"];
                ViewBag.SiteName =
                    config["SiteName"] ?? "দুর্জয় বিডি নিউজ ২৪";
            }
        }
        catch
        {
            ViewBag.BreakingNews = new List<object>();
            ViewBag.Categories = new List<object>();
        }
        base.OnActionExecuting(ctx);
    }
}