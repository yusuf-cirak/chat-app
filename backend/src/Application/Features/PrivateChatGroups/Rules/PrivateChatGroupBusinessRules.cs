using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Rules;

namespace Application.Features.PrivateChatGroups.Rules;

public sealed class PrivateChatGroupBusinessRules : BaseBusinessRules
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    internal PrivateChatGroupBusinessRules(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    internal void VerifyUserIdBeforeCreatingPrivateChatGroup(string userId)
    {
        var currentUserId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId is null || currentUserId != userId)
        {
            throw new ForbiddenAccessException("You are not allowed to create private chat group for other users");
        }
    }
}