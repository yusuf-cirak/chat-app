using Application.Abstractions.Services.Image;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Image;

public sealed class Image : IImage
{
    private readonly IImageService _imageService;

    public Image(IImageService imageService)
    {
        _imageService = imageService;
    }

    public Task<string> UploadImageAsync(string userId, IFormFile file)
    {
        return _imageService.UploadImageAsync(userId, file);
    }

    public Task<bool> DeleteImageAsync(string userId)
    {
        return _imageService.DeleteImageAsync(userId);
    }
}