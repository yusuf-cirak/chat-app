using FluentValidation;

namespace Application.Features.Users.Commands.UploadImage;

public sealed class UploadImageCommandValidator : AbstractValidator<UploadImageCommandRequest>
{
    public UploadImageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.File).NotNull().NotEmpty().WithMessage("Image file is required");
    }
}