using Application.Abstractions.Security;
using Application.Abstractions.Services.Chat;
using MongoDB.Driver;

namespace Application.Features.Users.Queries.GetOnlineChatUsers;

public readonly record struct GetOnlineChatUsersQueryRequest() : IRequest<List<string>>, ISecuredRequest;

public sealed class GetOnlineChatUsersQueryHandler : IRequestHandler<GetOnlineChatUsersQueryRequest, List<string>>
{
    private readonly IChatService _chatService;
    private readonly IMongoService _mongoService;

    public GetOnlineChatUsersQueryHandler(IChatService chatService, IMongoService mongoService)
    {
        _chatService = chatService;
        _mongoService = mongoService;
    }

    public Task<List<string>> Handle(GetOnlineChatUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var onlineUserIds = _chatService.GetOnlineUsers();

        var userProjection = Builders<User>.Projection.Expression(user => user.UserName);
        var userNames = _mongoService.GetCollection<User>().Find(e => onlineUserIds.Contains(e.Id))
            .Project(userProjection).ToList();

        return Task.FromResult(userNames);
    }
}