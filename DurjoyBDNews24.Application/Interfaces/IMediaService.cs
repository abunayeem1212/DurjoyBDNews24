namespace DurjoyBDNews24.Application.Interfaces;

public interface IMediaService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder);
    Task<bool> DeleteImageAsync(string fileUrl);
    bool IsValidImage(string fileName, long fileSizeBytes);
}