using DurjoyBDNews24.Application.DTOs.Video;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DurjoyBDNews24.Web.Controllers;

public class VideoController(
    IApiService api,
    IConfiguration config) : BaseController(api, config)
{
    public async Task<IActionResult> Index(
        [FromQuery] int page = 1)
    {
        var videos = await api.GetVideosAsync(page, 12);
        var featured = await api.GetFeaturedVideosAsync();

        ViewBag.Videos = videos;
        ViewBag.FeaturedVideos = featured;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = videos.TotalPages;

        return View();
    }

    public async Task<IActionResult> Watch(int id)
    {
        var video = await api.GetVideoByIdAsync(id);
        if (video is null)
            return RedirectToAction(nameof(Index));

        var related = await api.GetFeaturedVideosAsync();
        ViewBag.Video = video;
        ViewBag.RelatedVideos = related
            .Where(v => v.Id != id).Take(5);

        return View();
    }

    public async Task<IActionResult> Live()
    {
        var channels = await api.GetLiveTVChannelsAsync();
        ViewBag.Channels = channels;
        ViewBag.LiveChannel = channels
            .FirstOrDefault(c => c.IsLive);

        return View();
    }
}