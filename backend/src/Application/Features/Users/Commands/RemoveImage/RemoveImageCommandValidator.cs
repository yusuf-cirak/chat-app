using FluentValidation;

namespace Application.Features.Users.Commands.RemoveImage;

public sealed class RemoveImageCommandValidator : AbstractValidator<RemoveImageCommandRequest>
{
    public RemoveImageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}