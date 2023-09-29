using System.Security.Claims;
using Application.Abstractions.Security;
using MongoDB.Driver;

namespace Application.Features.Messages.Queries.GetChatGroupMessages;

public readonly record struct GetChatGroupMessagesQueryRequest
    () : IRequest<Dictionary<ObjectId, List<Message>>>, ISecuredRequest;

public sealed class
    GetChatGroupMessagesQueryHandler : IRequestHandler<GetChatGroupMessagesQueryRequest,
        Dictionary<ObjectId, List<Message>>>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetChatGroupMessagesQueryHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Dictionary<ObjectId, List<Message>>> Handle(GetChatGroupMessagesQueryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = ObjectId.Parse(_httpContextAccessor.HttpContext?.User.Claims
            .First(e => e.Type == ClaimTypes.NameIdentifier).Value);

        // Getting the userChatGroups with only Id field
        var userChatGroupProjection = Builders<ChatGroup>.Projection
            .Include(e => e.Id);

        var userChatGroups = _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId))
            .Project<ObjectId>(userChatGroupProjection).ToList();


        var messages = _mongoService.GetCollection<Message>().Find(e => userChatGroups.Contains(e.ChatGroupId)).ToList()
            .GroupBy(e => e.ChatGroupId).ToDictionary(e => e.Key, e => e.ToList());

        return Task.FromResult(messages);
    }
}