using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Features.Auths.Dtos;
using Application.Features.Auths.Rules;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Auths.Commands.Register;

public readonly record struct RegisterCommandRequest(string UserName, string Password) : IRequest<TokenResponseDto>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterCommandRequest, TokenResponseDto>
{
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;
    private readonly IHashingHelper _hashingHelper;

    private readonly IMongoService _mongoService;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegisterUserCommandHandler(IMongoService mongoService, AuthBusinessRules authBusinessRules,
        IJwtHelper jwtHelper, IHashingHelper hashingHelper, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _authBusinessRules = authBusinessRules;
        _jwtHelper = jwtHelper;
        _hashingHelper = hashingHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TokenResponseDto> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await _authBusinessRules.UserNameCannotBeDuplicatedBeforeRegistered(request.UserName);

        _hashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        User newUser = new()
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        var userIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        AccessToken accessToken = _jwtHelper.CreateAccessToken(newUser);
        RefreshToken refreshToken = _jwtHelper.CreateRefreshToken(newUser,userIpAddress);
        
        var tasks = new List<Task>(2);
        
        tasks.Add(_mongoService.GetCollection<User>().InsertOneAsync(newUser, cancellationToken: default));

        tasks.Add(_mongoService.GetCollection<RefreshToken>().InsertOneAsync(refreshToken, cancellationToken: default));

        await Task.WhenAll(tasks);
        
        return new TokenResponseDto(accessToken.Token, refreshToken.Token);
    }
}