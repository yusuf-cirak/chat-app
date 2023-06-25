using Application.Features.Auths.Rules;
using Domain.Entities;
using Infrastructure.Hashing;
using Infrastructure.JWT;
using Infrastructure.Persistence.Services;
using MediatR;

namespace Application.Features.Auths.Commands.Register;

public record RegisterCommandRequest(string UserName,string Password):IRequest<AccessToken>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterCommandRequest, AccessToken>
{
    
    private readonly IMongoService<User> _mongoService;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;

    public RegisterUserCommandHandler(IMongoService<User> mongoService, AuthBusinessRules authBusinessRules, IJwtHelper jwtHelper)
    {
        _mongoService = mongoService;
        _authBusinessRules = authBusinessRules;
        _jwtHelper = jwtHelper;
    }

    public async Task<AccessToken> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await _authBusinessRules.UserNameCannotBeDuplicatedBeforeRegistered(request.UserName);
        
        HashingHelper.CreatePasswordHash(request.Password,out byte[] passwordHash,out byte[] passwordSalt);
        
        var newUser = new User
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
        
        await _mongoService.Collection.InsertOneAsync(newUser, cancellationToken: cancellationToken);

        return _jwtHelper.CreateToken(newUser);
    }
}