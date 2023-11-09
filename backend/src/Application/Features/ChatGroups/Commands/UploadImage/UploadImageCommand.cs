using Application.Abstractions.Security;
using Application.Abstractions.Services.Image;
using Application.Common.Extensions;
using Application.Features.ChatGroups.Rules;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Commands.UploadImage;

public sealed class UploadChatGroupImageCommandRequest : IRequest<string>, ISecuredRequest
{
    public string ChatGroupId { get; set; }
    public IFormFile File { get; set; }

    public UploadChatGroupImageCommandRequest()
    {
    }

    public UploadChatGroupImageCommandRequest(string chatGroupId, IFormFile file)
    {
        ChatGroupId = chatGroupId;
        File = file;
    }
}

public sealed class UploadChatGroupImageCommandHandler : IRequestHandler<UploadChatGroupImageCommandRequest, string>
{
    private readonly ChatGroupBusinessRules _chatGroupBusinessRules;
    private readonly IImageService _imageService;
    private readonly IMongoService _mongoService;
    private readonly ILogger<UploadChatGroupImageCommandRequest> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadChatGroupImageCommandHandler(ChatGroupBusinessRules chatGroupBusinessRules, IImageService imageService,
        IMongoService mongoService, ILogger<UploadChatGroupImageCommandRequest> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _chatGroupBusinessRules = chatGroupBusinessRules;
        _imageService = imageService;
        _mongoService = mongoService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(UploadChatGroupImageCommandRequest request, CancellationToken cancellationToken)
    {
        await _chatGroupBusinessRules.UserMustExistInChatGroupBeforeUploadingImage(request.ChatGroupId);

        var imagePublicId = await _imageService.UploadImageAsync(request.ChatGroupId, request.File);

        var chatGroupUpdateResult = await _mongoService.GetCollection<ChatGroup>()
            .UpdateOneAsync(cg => cg.Id == request.ChatGroupId,
                Builders<ChatGroup>.Update.Set(chatGroup => chatGroup.ProfileImageUrl, imagePublicId),
                cancellationToken: cancellationToken);

        if (!chatGroupUpdateResult.IsModifiedCountAvailable)
        {
            _logger.LogError("{RequestName} - Failed to update chat group image for {Username} and {ChatGroupId}",
                nameof(UploadChatGroupImageCommandRequest),
                _httpContextAccessor.HttpContext.User.GetUsername(),
                request.ChatGroupId);
            await _imageService.DeleteImageAsync(imagePublicId);
            return string.Empty;
        }

        return imagePublicId;
    }
}