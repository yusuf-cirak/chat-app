using System.Security.Claims;
using Application.Common.Rules;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Rules;

public sealed class ChatGroupBusinessRules : BaseBusinessRules
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMongoService _mongoService;

    public ChatGroupBusinessRules(IHttpContextAccessor httpContextAccessor, IMongoService mongoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _mongoService = mongoService;
    }

    public void UserMustExistInParticipantsBeforeCreatingChatGroup(List<string> participants)
    {
        var userId = (_httpContextAccessor.HttpContext.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value);
        
        if (!participants.Contains(userId))
        {
            throw new ForbiddenAccessException("You are not allowed to create chat group for other users");
        }
    }

    public async Task<ChatGroup> UserMustExistInChatGroupBeforeUploadingImage(string requestChatGroupId)
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.First(claim=>claim.Type == ClaimTypes.NameIdentifier).Value;
        
        var chatGroup = await _mongoService.GetCollection<ChatGroup>().Find(cg => cg.Id == requestChatGroupId)
            .SingleOrDefaultAsync();
        
        if (!chatGroup.UserIds.Contains(userId))
        {
            throw new BusinessException("You are not allowed to upload image for this chat group");
        }
        
        return chatGroup;
    }
}