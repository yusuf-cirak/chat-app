using Application.Abstractions.Security;
using Application.Abstractions.Services.Image;
using Application.Features.Users.Rules;
using MongoDB.Driver;

namespace Application.Features.Users.Commands.UploadImage;

public sealed class UploadImageCommandRequest : IRequest<string>, ISecuredRequest
{
    public string UserId { get; set; }
    public IFormFile File { get; set; }

    public UploadImageCommandRequest()
    {
    }

    public UploadImageCommandRequest(string userId, IFormFile file)
    {
        UserId = userId;
        File = file;
    }
}

public sealed class UploadImageCommandHandler : IRequestHandler<UploadImageCommandRequest, string>
    {
        private readonly IImageService _imageService;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IMongoService _mongoService;

        public UploadImageCommandHandler(IImageService imageService, UserBusinessRules userBusinessRules, IMongoService mongoService)
        {
            _imageService = imageService;
            _userBusinessRules = userBusinessRules;
            _mongoService = mongoService;
        }

        public async Task<string> Handle(UploadImageCommandRequest request, CancellationToken cancellationToken)
        {
            _userBusinessRules.ValidateUserBeforeUploadingOrRemovingImage(request.UserId);

            var imagePublicId = await _imageService.UploadImageAsync(request.UserId, request.File);

           var updateUserResult = await _mongoService.GetCollection<User>().UpdateOneAsync(user => user.Id == request.UserId,
                Builders<User>.Update.Set(user => user.ProfileImageUrl, imagePublicId), cancellationToken: cancellationToken);

           if (!updateUserResult.IsModifiedCountAvailable)
           {
               await _imageService.DeleteImageAsync(imagePublicId);
               return string.Empty;
           }
           
           return imagePublicId;
        }
    }