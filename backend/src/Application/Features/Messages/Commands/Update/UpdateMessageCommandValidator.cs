using FluentValidation;

namespace Application.Features.Messages.Commands.Update;

public sealed class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommandRequest>
{
    public UpdateMessageCommandValidator()
    {
        RuleFor(e => e.Body).NotEmpty().WithMessage("Body cannot be empty").MaximumLength(1000)
            .WithMessage("Body cannot be longer than 1000 characters");
        
        RuleFor(e => e.Id).NotEmpty().WithMessage("Id cannot be empty");
    }
}