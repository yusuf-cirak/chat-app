using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Common.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.Messages.Queries.GetChatGroupMessages;

public readonly record struct GetChatGroupMessagesQueryRequest
    () : IRequest<Dictionary<string, List<Message>>>, ISecuredRequest;

public sealed class
    GetChatGroupMessagesQueryHandler : IRequestHandler<GetChatGroupMessagesQueryRequest,
        Dictionary<string, List<Message>>>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetChatGroupMessagesQueryRequest> _logger;

    public GetChatGroupMessagesQueryHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor,
        ILogger<GetChatGroupMessagesQueryRequest> logger)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Task<Dictionary<string, List<Message>>> Handle(GetChatGroupMessagesQueryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.Claims
            .First(e => e.Type == ClaimTypes.NameIdentifier).Value);

        // Getting the userChatGroups with only Id field

        var chatGroupProjection = Builders<ChatGroup>.Projection.Include(e => e.Id);

        var userChatGroupIds =
            _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId!))
                .Project<ChatGroup>(chatGroupProjection).ToList().Select(cg => cg.Id).ToList();

        var messages = _mongoService.GetCollection<Message>()
            .Find(message => userChatGroupIds.Contains(message.ChatGroupId)).ToList()
            .GroupBy(e => e.ChatGroupId).ToDictionary(e => e.Key, e => e.ToList());

        _logger.LogInformation("{RequestName} - {MessageCount} of messages retrieved for {UserId} - {Username}",
            nameof(GetChatGroupMessagesQueryRequest),
            messages.Count,
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername());

        return Task.FromResult(messages);
    }
}