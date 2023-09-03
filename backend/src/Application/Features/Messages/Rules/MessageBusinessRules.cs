using Application.Common.Rules;

namespace Application.Features.Messages.Rules;

public sealed class MessageBusinessRules : BaseBusinessRules
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MessageBusinessRules(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}