using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Common.Rules;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.Auths.Rules;

public sealed class AuthBusinessRules : BaseBusinessRules
{
    
    // FromServices attribute could be used instead of constructor injection
    private readonly IMongoService _mongoService;
    private readonly IHashingHelper _hashingHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthBusinessRules(IMongoService mongoService, IHashingHelper hashingHelper, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _hashingHelper = hashingHelper;
        _httpContextAccessor = httpContextAccessor;
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
            throw new BusinessException("Wrong credentials");
        }
    }

    public async Task<User> UserShouldExistBeforeLogin(string userName)
    {
        User? user = await _mongoService.GetCollection<User>().Find(u => u.UserName == userName).FirstOrDefaultAsync();

        if (user is null)
        {
            throw new BusinessException("There is no user with that user name");
        }

        return user;
    }
    
    public async Task GetAndVerifyUserRefreshToken(ObjectId userId,string refreshTokenFromRequest)
    {
        var refreshTokenCollection = _mongoService.GetCollection<RefreshToken>();
        
        var refreshToken = await _mongoService.GetCollection<RefreshToken>()
            .Find(rt => rt.UserId == userId)
            .SortByDescending(e=>e.Id)
            .FirstOrDefaultAsync(cancellationToken: default);
        
        var address = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        
        if (refreshToken is null ||refreshToken.Token !=refreshTokenFromRequest || refreshToken.CreatedByIp != address || refreshToken.ExpiresAt < DateTime.Now)
        {
            throw new BusinessException("Refresh token is not valid");
        }
    }
}