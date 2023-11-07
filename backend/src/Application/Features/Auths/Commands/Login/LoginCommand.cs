using Application.Abstractions.Helpers;
using Application.Features.Auths.Dtos;
using Application.Features.Auths.Rules;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.Auths.Commands.Login;

public readonly record struct LoginCommandRequest(string UserName, string Password) : IRequest<TokenResponseDto>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommandRequest, TokenResponseDto>
{
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IJwtHelper jwtHelper, AuthBusinessRules authBusinessRules, IMongoService mongoService,
        IHttpContextAccessor httpContextAccessor, ILogger<LoginCommandHandler> logger)
    {
        _jwtHelper = jwtHelper;
        _authBusinessRules = authBusinessRules;
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TokenResponseDto> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        User user = await _authBusinessRules.UserNameShouldExistBeforeLogin(request.UserName);

        _authBusinessRules.UserCredentialsMustMatchBeforeLogin(request.Password, user.PasswordHash, user.PasswordSalt);

        var userIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        AccessToken accessToken = _jwtHelper.CreateAccessToken(user);
        RefreshToken refreshToken = _jwtHelper.CreateRefreshToken(user, userIpAddress);


        var refreshTokenCollection = _mongoService.GetCollection<RefreshToken>();

        await refreshTokenCollection.DeleteManyAsync(rt => rt.UserId == user.Id, cancellationToken: default);

        await refreshTokenCollection.InsertOneAsync(refreshToken, cancellationToken: default);

        _logger.LogInformation("{RequestName} - Access token and refresh token created for {UserId} - {Username}",
            nameof(LoginCommandRequest),
            user.Id,
            user.UserName);

        return new TokenResponseDto(accessToken.Token, refreshToken.Token);
    }
}