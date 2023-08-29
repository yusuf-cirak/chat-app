using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Common.Rules;
using Domain.Entities;
using MongoDB.Driver;

namespace Application.Features.Auths.Rules;

public sealed class AuthBusinessRules : BaseBusinessRules
{
    private readonly IMongoService _mongoService;
    private readonly IHashingHelper _hashingHelper;

    public AuthBusinessRules(IMongoService mongoService, IHashingHelper hashingHelper)
    {
        _mongoService = mongoService;
        _hashingHelper = hashingHelper;
    }

    public async Task UserNameCannotBeDuplicatedBeforeRegistered(string userName)
    {
        User? user = await _mongoService.GetCollection<User>().Find(u => u.UserName == userName).FirstOrDefaultAsync();

        if (user != null)
        {
            throw new BusinessException("A user already exists with that email");
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
}