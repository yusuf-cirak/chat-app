using System.Security.Claims;
using Application.Abstractions.Security;

namespace Application.Features.Messages.Commands.Create;

public readonly record struct CreateMessageCommandRequest(string Content, string ChatGroupId) : IRequest<string>,ISecuredRequest;

public sealed class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest, string>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateMessageCommandHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.Claims.First(e=>e.Type==ClaimTypes.NameIdentifier).Value;

        var message = new Message(new ObjectId(userId).ToString()!, request.ChatGroupId, request.Content, sentAt:DateTime.Now);
        
        await _mongoService.GetCollection<Message>().InsertOneAsync(message, cancellationToken: cancellationToken);
        
        return message.Id;
    }
}