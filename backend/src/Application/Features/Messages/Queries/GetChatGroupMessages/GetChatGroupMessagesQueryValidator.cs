using FluentValidation;

namespace Application.Features.Messages.Queries.GetChatGroupMessages;

public class GetChatGroupMessagesQueryValidator : AbstractValidator<GetChatGroupMessagesQueryRequest>
{
    public GetChatGroupMessagesQueryValidator()
    {
        RuleFor(e => e.ChatGroupIds).Must(e => e.Count > 0).WithMessage("Chat group ids cannot be empty");
    }
    
}