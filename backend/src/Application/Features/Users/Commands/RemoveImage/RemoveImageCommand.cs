using Application.Abstractions.Security;
using Application.Abstractions.Services.Image;
using Application.Features.Users.Rules;

namespace Application.Features.Users.Commands.RemoveImage;

public readonly record struct RemoveImageCommandRequest(string UserId) : IRequest<bool>,ISecuredRequest;

public sealed class RemoveImageCommandHandler : IRequestHandler<RemoveImageCommandRequest,bool>
{
    private readonly IImageService _imageService;
    private readonly UserBusinessRules _userBusinessRules;

    public RemoveImageCommandHandler(IImageService imageService, UserBusinessRules userBusinessRules)
    {
        _imageService = imageService;
        _userBusinessRules = userBusinessRules;
    }

    public async Task<bool> Handle(RemoveImageCommandRequest request, CancellationToken cancellationToken)
    {
        _userBusinessRules.ValidateUserBeforeUploadingOrRemovingImage(request.UserId);

        var imageRemoveResult = await _imageService.DeleteImageAsync(request.UserId);

        return imageRemoveResult;
    }
}