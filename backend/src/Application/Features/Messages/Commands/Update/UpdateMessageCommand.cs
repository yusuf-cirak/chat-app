using System.Security.Claims;
using Application.Abstractions.Security;
using MongoDB.Driver;

namespace Application.Features.Messages.Commands.Update;

public readonly record struct UpdateMessageCommandRequest(ObjectId Id, string Body) : IRequest<bool>,ISecuredRequest;

public sealed class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommandRequest, bool>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateMessageCommandHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(UpdateMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Message>.Update.Set(e => e.Body, request.Body);
        
        var userId = _httpContextAccessor.HttpContext.User.Claims.First(claim=>claim.Type==ClaimTypes.NameIdentifier).Value;
        
        var result = (await _mongoService.GetCollection<Message>().UpdateOneAsync(e => e.Id == request.Id && e.UserId==ObjectId.Parse(userId), updateDefinition, cancellationToken: cancellationToken)).IsModifiedCountAvailable;
        
        return result;
    }
}