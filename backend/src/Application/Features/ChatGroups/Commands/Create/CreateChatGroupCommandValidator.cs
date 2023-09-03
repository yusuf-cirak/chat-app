using FluentValidation;

namespace Application.Features.ChatGroups.Commands.Create;

public sealed class CreateChatGroupCommandValidator : AbstractValidator<CreateChatGroupCommandRequest>
{
    public CreateChatGroupCommandValidator()
    {
        RuleFor(e => e.ParticipantUserIds).Must(e => e.Count > 0).WithMessage("Participant user ids cannot be empty");
    }
}