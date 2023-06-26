using Application.Features.Auths.Rules;
using Application.Services.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auths.Commands.Register;

public readonly record struct RegisterCommandRequest(string UserName, string Password) : IRequest<string>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterCommandRequest, string>
{
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;

    private readonly IMongoService<User> _mongoService;

    public RegisterUserCommandHandler(IMongoService<User> mongoService, AuthBusinessRules authBusinessRules,
        IJwtHelper jwtHelper)
    {
        _mongoService = mongoService;
        _authBusinessRules = authBusinessRules;
        _jwtHelper = jwtHelper;
    }

    public async Task<string> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await _authBusinessRules.UserNameCannotBeDuplicatedBeforeRegistered(request.UserName);

        HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var newUser = new User
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _mongoService.Collection.InsertOneAsync(newUser, cancellationToken: cancellationToken);

        return _jwtHelper.CreateToken(newUser).Token;
    }
}