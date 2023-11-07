using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Common.Extensions;
using Application.Features.Users.Dtos;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.Users.Queries.GetAllChatUsers;

public readonly record struct GetAllChatUsersQueryRequest() : IRequest<List<GetUserDto>>, ISecuredRequest;

public sealed class GetAllChatUsersQueryRequestHandler : IRequestHandler<GetAllChatUsersQueryRequest, List<GetUserDto>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMongoService _mongoService;
    private readonly ILogger<GetAllChatUsersQueryRequest> _logger;

    public GetAllChatUsersQueryRequestHandler(IHttpContextAccessor httpContextAccessor, IMongoService mongoService,
        ILogger<GetAllChatUsersQueryRequest> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _mongoService = mongoService;
        _logger = logger;
    }

    public Task<List<GetUserDto>> Handle(GetAllChatUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var userId = (_httpContextAccessor.HttpContext.User.Claims
            .First(e => e.Type == ClaimTypes.NameIdentifier).Value);

        var chatGroupProjection = Builders<ChatGroup>.Projection
            .Expression(cg => cg.UserIds);

        var userIds = _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId))
            .Project(chatGroupProjection).ToList().SelectMany(cg => cg).ToList();

        var userProjection = Builders<User>.Projection
            .Include(e => e.Id)
            .Include(e => e.UserName)
            .Include(u => u.ProfileImageUrl);

        var chatGroupUsers = _mongoService.GetCollection<User>()
            .Find(e => userIds.Contains(e.Id)).Project<GetUserDto>(userProjection)
            .ToList();

        _logger.LogInformation("{RequestName} - {UserCount} of chat users retrieved for {UserId} - {Username}",
            nameof(GetAllChatUsersQueryRequest),
            chatGroupUsers.Count,
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername()
        );

        return Task.FromResult(chatGroupUsers);
    }
}