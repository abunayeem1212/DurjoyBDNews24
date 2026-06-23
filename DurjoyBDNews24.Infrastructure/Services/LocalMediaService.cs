using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace DurjoyBDNews24.Infrastructure.Services;

public class LocalMediaService(
    IWebHostEnvironment env,
    ILogger<LocalMediaService> logger) : IMediaService
{
    private static readonly string[] _allowedExtensions =
        [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<string> UploadImageAsync(
        Stream imageStream, string fileName, string folder)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var uploadFolder = Path.Combine(env.WebRootPath, "uploads", folder);

        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        var filePath = Path.Combine(uploadFolder, uniqueName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await imageStream.CopyToAsync(fileStream);

        logger.LogInformation("Image uploaded: {Path}", filePath);
        return $"/uploads/{folder}/{uniqueName}";
    }

    public Task<bool> DeleteImageAsync(string fileUrl)
    {
        try
        {
            var filePath = Path.Combine(
                env.WebRootPath,
                fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete image failed: {Url}", fileUrl);
            return Task.FromResult(false);
        }
    }

    public bool IsValidImage(string fileName, long fileSizeBytes)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return _allowedExtensions.Contains(ext) && fileSizeBytes <= MaxFileSizeBytes;
    }
}