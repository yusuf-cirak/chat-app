using System.Security.Claims;
using Application.Common.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Rules;

public sealed class UserBusinessRules : BaseBusinessRules
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserBusinessRules> _logger;

    public UserBusinessRules(IHttpContextAccessor httpContextAccessor, ILogger<UserBusinessRules> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public void ValidateUserBeforeUploadingOrRemovingImage(string userId)
    {
        var userIdFromToken = _httpContextAccessor.HttpContext.User.Claims
            .SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value!;

        if (userIdFromToken != userId)
        {
            _logger.LogCritical("User {UserId} tried to upload or remove image for user {UserIdFromToken}", userId,
                userIdFromToken);
            throw new BusinessException("You are not allowed to upload or remove image for this user");
        }
    }
}