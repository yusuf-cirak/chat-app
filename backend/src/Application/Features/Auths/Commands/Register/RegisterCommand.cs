using Application.Abstractions.Helpers;
using Application.Abstractions.Services;
using Application.Features.Auths.Rules;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auths.Commands.Register;

public readonly record struct RegisterCommandRequest(string UserName, string Password) : IRequest<string>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterCommandRequest, string>
{
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;
    private readonly IHashingHelper _hashingHelper;

    private readonly IMongoService _mongoService;

    public RegisterUserCommandHandler(IMongoService mongoService, AuthBusinessRules authBusinessRules,
        IJwtHelper jwtHelper, IHashingHelper hashingHelper)
    {
        _mongoService = mongoService;
        _authBusinessRules = authBusinessRules;
        _jwtHelper = jwtHelper;
        _hashingHelper = hashingHelper;
    }

    public async Task<string> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await _authBusinessRules.UserNameCannotBeDuplicatedBeforeRegistered(request.UserName);

        _hashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var newUser = new User
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _mongoService.GetCollection<User>().InsertOneAsync(newUser, cancellationToken: default);

        return _jwtHelper.CreateToken(newUser).Token;
    }
}