using Application.Features.Auths.Rules;
using Application.Services.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auths.Commands.Login;

public readonly record struct LoginCommandRequest(string UserName, string Password) : IRequest<string>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommandRequest, string>
{
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IJwtHelper _jwtHelper;

    public LoginCommandHandler(IJwtHelper jwtHelper, AuthBusinessRules authBusinessRules)
    {
        _jwtHelper = jwtHelper;
        _authBusinessRules = authBusinessRules;
    }

    public async Task<string> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        User? user = await _authBusinessRules.UserShouldExistBeforeLogin(request.UserName);

        _authBusinessRules.UserCredentialsMustMatchBeforeLogin(request.Password, user.PasswordHash, user.PasswordSalt);

        return _jwtHelper.CreateToken(user).Token;
    }
}