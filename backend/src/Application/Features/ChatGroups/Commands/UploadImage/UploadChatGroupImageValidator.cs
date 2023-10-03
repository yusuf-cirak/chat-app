using FluentValidation;

namespace Application.Features.ChatGroups.Commands.UploadImage;

public sealed class UploadChatGroupImageValidator : AbstractValidator<UploadChatGroupImageCommandRequest>
{
    public UploadChatGroupImageValidator()
    {
        RuleFor(u => u.ChatGroupId).NotEmpty().WithMessage("ChatGroupId cannot be empty");
        RuleFor(u => u.File).NotNull().WithMessage("Image cannot be null");
    }
}