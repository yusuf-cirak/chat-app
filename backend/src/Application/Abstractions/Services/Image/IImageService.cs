namespace Application.Abstractions.Services.Image;

public interface IImageService
{
    Task<string> UploadImageAsync(string id, IFormFile file);
    Task<bool> DeleteImageAsync(string id);
}