using Application.Abstractions.Security;
using Application.Abstractions.Services.Image;
using Application.Features.ChatGroups.Rules;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Commands.UploadImage;
public sealed class UploadChatGroupImageCommandRequest : IRequest<string>,ISecuredRequest
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

public sealed class UploadChatGroupImageCommandHandler : IRequestHandler<UploadChatGroupImageCommandRequest,string>
{
    private readonly ChatGroupBusinessRules _chatGroupBusinessRules;
    private readonly IImageService _imageService;
    private readonly IMongoService _mongoService;

    public UploadChatGroupImageCommandHandler(ChatGroupBusinessRules chatGroupBusinessRules, IImageService imageService, IMongoService mongoService)
    {
        _chatGroupBusinessRules = chatGroupBusinessRules;
        _imageService = imageService;
        _mongoService = mongoService;
    }

    public async Task<string> Handle(UploadChatGroupImageCommandRequest request, CancellationToken cancellationToken)
    {
        await _chatGroupBusinessRules.UserMustExistInChatGroupBeforeUploadingImage(request.ChatGroupId);

       var imagePublicId = await _imageService.UploadImageAsync(request.ChatGroupId, request.File);

       var chatGroupUpdateResult =await  _mongoService.GetCollection<ChatGroup>().UpdateOneAsync(cg => cg.Id == request.ChatGroupId,Builders<ChatGroup>.Update.Set(chatGroup => chatGroup.ImageUrl, imagePublicId), cancellationToken: cancellationToken);

       if (!chatGroupUpdateResult.IsModifiedCountAvailable)
       {
           await _imageService.DeleteImageAsync(imagePublicId);
           return string.Empty;
       }

       return imagePublicId;
    }
}