namespace Application.Abstractions.Services.Image;

public interface IImage
{
    Task<string> UploadImageAsync(string userId, IFormFile file);
    Task<bool> DeleteImageAsync(string userId);
}