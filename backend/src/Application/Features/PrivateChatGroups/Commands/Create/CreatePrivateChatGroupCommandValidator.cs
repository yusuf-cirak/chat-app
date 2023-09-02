namespace Application.Features.PrivateChatGroups.Commands.Create;
using FluentValidation;

public sealed class CreatePrivateChatGroupCommandValidator : AbstractValidator<CreatePrivateChatGroupCommandRequest>
{
    public CreatePrivateChatGroupCommandValidator()
    {
        RuleFor(r=>r.UserId).NotEmpty().WithMessage("User id cannot be empty");
        RuleFor(r=>r.ParticipantUserId).NotEmpty().WithMessage("Participant user id cannot be empty");
    }
}

