using FluentValidation;

namespace Application.Features.Auths.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommandRequest>
{
    public RegisterCommandValidator()
    {
        RuleFor(u => u.UserName).NotEmpty().WithMessage("First name cannot be empty");
        RuleFor(u => u.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be longer than 6 characters");
    }
}