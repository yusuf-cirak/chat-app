using FluentValidation;

namespace Application.Features.Messages.Commands.Create;

public sealed class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommandRequest>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(e=>e.ChatGroupId).NotEmpty().WithMessage("ChatGroupId is required");
        RuleFor(e=>e.Content).NotEmpty().WithMessage("Content is required").MaximumLength(1000).WithMessage("Content must be less than 1000 characters");
    }
}