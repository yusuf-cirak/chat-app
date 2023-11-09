using Application.Abstractions.Security;
using Application.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TRequest> _logger;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, ILogger<TRequest> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var userClaimsCount = _httpContextAccessor.HttpContext?.User?.Claims?.Count() ?? 0;

        if (userClaimsCount == 0)
        {
            throw new UnauthorizedAccessException("You are not authorized to access this resource.");
        }

        _logger.LogInformation("{RequestName} - {UserId} {Username} authenticated for request", nameof(TRequest),
            _httpContextAccessor.HttpContext?.User?.GetUserId(), _httpContextAccessor.HttpContext?.User?.GetUsername());

        // TODO: Role based authorization

        TResponse response = await next();
        return response;
    }
}