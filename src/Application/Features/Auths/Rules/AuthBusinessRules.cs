using Application.Common.Exceptions;
using Application.Common.Rules;
using Application.Services.Abstractions;
using Domain.Entities;
using MongoDB.Driver;

namespace Application.Features.Auths.Rules;

public sealed class AuthBusinessRules : BaseBusinessRules
{
    private readonly IMongoService<User> _mongoService;

    public AuthBusinessRules(IMongoService<User> mongoService)
    {
        _mongoService = mongoService;
    }

    public async Task UserNameCannotBeDuplicatedBeforeRegistered(string userName)
    {
        User? user = await _mongoService.Collection.Find(u => u.UserName == userName).FirstOrDefaultAsync();

        if (user != null)
        {
            throw new BusinessException("A user already exists with that email");
        }
    }


    public void UserCredentialsMustMatchBeforeLogin(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        if (!HashingHelper.VerifyPasswordHash(password, passwordHash, passwordSalt))
        {
            throw new BusinessException("Wrong credentials");
        }
    }

    public async Task<User> UserShouldExistBeforeLogin(string userName)
    {
        User? user = await _mongoService.Collection.Find(u => u.UserName == userName).FirstOrDefaultAsync();

        if (user is null)
        {
            throw new BusinessException("There is no user with that user name");
        }

        return user;
    }
}