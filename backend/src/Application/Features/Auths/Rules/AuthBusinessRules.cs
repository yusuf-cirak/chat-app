using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Rules;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.Auths.Rules;

public sealed class AuthBusinessRules : BaseBusinessRules
{
    // FromServices attribute could be used instead of constructor injection
    private readonly IMongoService _mongoService;
    private readonly IHashingHelper _hashingHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthBusinessRules> _logger;

    public AuthBusinessRules(IMongoService mongoService, IHashingHelper hashingHelper,
        IHttpContextAccessor httpContextAccessor, ILogger<AuthBusinessRules> logger)
    {
        _mongoService = mongoService;
        _hashingHelper = hashingHelper;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task UserNameCannotBeDuplicatedBeforeRegistered(string userName)
    {
        User? user = await _mongoService.GetCollection<User>().Find(u => u.UserName == userName).FirstOrDefaultAsync();

        if (user != null)
        {
            throw new BusinessException("A user already exists with that username");
        }
    }


    public void UserCredentialsMustMatchBeforeLogin(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        if (!_hashingHelper.VerifyPasswordHash(password, passwordHash, passwordSalt))
        {
            var username = _httpContextAccessor.HttpContext.User.GetUsername();
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            _logger.LogInformation("Wrong credentials supplied for {UserId} {UserName}", userId,
                username);
            throw new BusinessException("Wrong credentials");
        }
    }

    public async Task<User> UserNameShouldExistBeforeLogin(string userName)
    {
        User? user = await _mongoService.GetCollection<User>().Find(u => u.UserName == userName).SingleOrDefaultAsync();

        if (user is null)
        {
            throw new BusinessException("There is no user with that user name");
        }

        return user;
    }

    public async Task<User> UserWithIdMustExistBeforeRefreshToken(string userId)
    {
        User? user = await _mongoService.GetCollection<User>().Find(u => u.Id == userId).SingleOrDefaultAsync();

        if (user is null)
        {
            _logger.LogCritical("{RequestName} - There is no user with that user id {UserId}", "RefreshTokenCommand",
                userId);
            throw new BusinessException("There is no user with that user id");
        }

        return user;
    }

    public async Task GetAndVerifyUserRefreshToken(string userId, string refreshTokenFromRequest)
    {
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        var refreshToken = await _mongoService.GetCollection<RefreshToken>()
            .Find(rt => rt.UserId == userId && rt.CreatedByIp == ipAddress)
            .SortByDescending(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken: default);

        var username = _httpContextAccessor.HttpContext.User.GetUsername();

        if (refreshToken is null)
        {
            _logger.LogCritical("{RequestName} - There is no refresh token for user {UserId}", "RefreshTokenCommand",
                userId);
        }


        else if (refreshToken.Token != refreshTokenFromRequest)
        {
            _logger.LogCritical("Refresh token is not valid for user {UserId} {Username}",
                userId, username);
        }

        else if (refreshToken.ExpiresAt < DateTime.Now)
        {
            _logger.LogInformation("Refresh token is expired for user {UserId} {Username}",
                userId, username);
        }
    }
}