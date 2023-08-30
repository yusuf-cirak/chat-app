using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Features.Auths.Dtos;
using Application.Features.Auths.Rules;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Auths.Commands.Login;

public readonly record struct LoginCommandRequest(string UserName, string Password) : IRequest<TokenResponseDto>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommandRequest, TokenResponseDto>
{
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginCommandHandler(IJwtHelper jwtHelper, AuthBusinessRules authBusinessRules, IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _jwtHelper = jwtHelper;
        _authBusinessRules = authBusinessRules;
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TokenResponseDto> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        User user = await _authBusinessRules.UserShouldExistBeforeLogin(request.UserName);

        _authBusinessRules.UserCredentialsMustMatchBeforeLogin(request.Password, user.PasswordHash, user.PasswordSalt);

        var userIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        AccessToken accessToken = _jwtHelper.CreateAccessToken(user);
        RefreshToken refreshToken = _jwtHelper.CreateRefreshToken(user,userIpAddress);
        
        var tasks = new List<Task>(2);
        
        tasks.Add(_mongoService.GetCollection<User>().InsertOneAsync(user, cancellationToken: default));

        tasks.Add(_mongoService.GetCollection<RefreshToken>().InsertOneAsync(refreshToken, cancellationToken: default));

        await Task.WhenAll(tasks);
        
        return new TokenResponseDto(accessToken.Token, refreshToken.Token);
    }
}