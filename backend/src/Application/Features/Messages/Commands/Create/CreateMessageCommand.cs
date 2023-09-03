using System.Security.Claims;
using Application.Abstractions.Security;

namespace Application.Features.Messages.Commands.Create;

public readonly record struct CreateMessageCommandRequest(string Content, ObjectId ChatGroupId) : IRequest<ObjectId>,ISecuredRequest;

public sealed class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest, ObjectId>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateMessageCommandHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ObjectId> Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.Claims.First(e=>e.Type==ClaimTypes.NameIdentifier).Value;

        var message = new Message(new ObjectId(userId), request.ChatGroupId, request.Content, sentAt:DateTime.Now);
        
        await _mongoService.GetCollection<Message>().InsertOneAsync(message, cancellationToken: cancellationToken);
        
        return message.Id;
    }
}