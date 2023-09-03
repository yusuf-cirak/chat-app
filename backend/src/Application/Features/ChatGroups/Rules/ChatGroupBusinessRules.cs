using System.Security.Claims;
using Application.Common.Rules;

namespace Application.Features.ChatGroups.Rules;

public sealed class ChatGroupBusinessRules : BaseBusinessRules
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatGroupBusinessRules(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void UserMustExistInParticipantsBeforeCreatingChatGroup(List<ObjectId> participants)
    {
        var userId = ObjectId.Parse(_httpContextAccessor.HttpContext.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value);
        
        if (!participants.Contains(userId))
        {
            throw new ForbiddenAccessException("You are not allowed to create chat group for other users");
        }
    }
}