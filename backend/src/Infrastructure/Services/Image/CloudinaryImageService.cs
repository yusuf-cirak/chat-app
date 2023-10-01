using Application.Abstractions.Services.Image;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Image;

public sealed class CloudinaryImageService : IImageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> UploadImageAsync(string userId, IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(userId, stream),
            Transformation = new Transformation().Height(50).Width(50).Crop("fill").Gravity("face"),
            Folder = "profile_images"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult.PublicId.Split('/')[^1];
    }

    public async Task<bool> DeleteImageAsync(string imagePublicId)
    {
        var deleteParams = new DeletionParams($"profile_images/{imagePublicId}")
        {
            ResourceType = ResourceType.Image,
            Type = "upload"
        };
        
        var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
        
        return deleteResult.Error == null;
    }
}