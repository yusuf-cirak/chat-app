using Application.Abstractions.Security;
using Application.Abstractions.Services.Chat;
using Application.Common.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.Users.Queries.GetOnlineChatUsers;

public readonly record struct GetOnlineChatUsersQueryRequest() : IRequest<List<string>>, ISecuredRequest;

public sealed class GetOnlineChatUsersQueryHandler : IRequestHandler<GetOnlineChatUsersQueryRequest, List<string>>
{
    private readonly IChatService _chatService;
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetOnlineChatUsersQueryRequest> _logger;

    public GetOnlineChatUsersQueryHandler(IChatService chatService, IMongoService mongoService,
        IHttpContextAccessor httpContextAccessor, ILogger<GetOnlineChatUsersQueryRequest> logger)
    {
        _chatService = chatService;
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Task<List<string>> Handle(GetOnlineChatUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var onlineUserIds = _chatService.GetOnlineUsers();

        var userProjection = Builders<User>.Projection.Expression(user => user.UserName);
        var userNames = _mongoService.GetCollection<User>().Find(e => onlineUserIds.Contains(e.Id))
            .Project(userProjection).ToList();

        _logger.LogInformation("{RequestName} - {UserCount} of online users retrieved for {UserId} - {Username}",
            nameof(GetOnlineChatUsersQueryRequest),
            userNames.Count,
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername());


        return Task.FromResult(userNames);
    }
}