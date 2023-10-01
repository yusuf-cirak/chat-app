using System.Security.Claims;
using Application.Abstractions.Helpers;
using Application.Abstractions.Security;
using Application.Features.Auths.Dtos;
using Application.Features.Auths.Rules;
using MongoDB.Driver;

namespace Application.Features.Auths.Commands.Refresh;

public readonly record struct RefreshTokenCommandRequest
    (string UserId, string RefreshToken) : IRequest<TokenResponseDto>;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, TokenResponseDto>
{
    private readonly IJwtHelper _jwtHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMongoService _mongoService;
    private readonly AuthBusinessRules _authBusinessRules;

    public RefreshTokenCommandHandler(IJwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor,
        IMongoService mongoService, AuthBusinessRules authBusinessRules)
    {
        _jwtHelper = jwtHelper;
        _httpContextAccessor = httpContextAccessor;
        _mongoService = mongoService;
        _authBusinessRules = authBusinessRules;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
    {
            var userId = (request.UserId);

            var user = await _authBusinessRules.UserWithIdMustExistBeforeRefreshToken(userId);

            await _authBusinessRules.GetAndVerifyUserRefreshToken(userId, request.RefreshToken);

            var userIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            var accessToken = _jwtHelper.CreateAccessToken(user);
            var refreshToken = _jwtHelper.CreateRefreshToken(user, userIpAddress);
            
            var refreshTokenCollection = _mongoService.GetCollection<RefreshToken>();

            // await refreshTokenCollection.DeleteManyAsync(rt => rt.UserId == userId, cancellationToken: default);

            await refreshTokenCollection.InsertOneAsync(refreshToken, cancellationToken: default);
        
            return new(accessToken.Token, refreshToken.Token);
        }
    }