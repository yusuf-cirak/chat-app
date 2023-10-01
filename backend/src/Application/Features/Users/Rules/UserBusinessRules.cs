using System.Security.Claims;
using Application.Common.Rules;

namespace Application.Features.Users.Rules;

public sealed class UserBusinessRules : BaseBusinessRules
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserBusinessRules(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void ValidateUserBeforeUploadingOrRemovingImage(string userId)
    {
        var userIdFromToken = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value!;

        if (userIdFromToken!=userId)
        {
            throw new BusinessException("You are not allowed to upload or remove image for this user");
        }
    }
}