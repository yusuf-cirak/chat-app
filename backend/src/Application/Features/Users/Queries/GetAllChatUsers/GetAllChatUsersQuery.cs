using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Features.ChatGroups.Dtos;
using Application.Features.Users.Dtos;
using MongoDB.Driver;

namespace Application.Features.Users.Queries.GetAllChatUsers;

public readonly record struct GetAllChatUsersQueryRequest() : IRequest<List<GetUserDto>>, ISecuredRequest;

public sealed class GetAllChatUsersQueryRequestHandler : IRequestHandler<GetAllChatUsersQueryRequest, List<GetUserDto>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMongoService _mongoService;

    public GetAllChatUsersQueryRequestHandler(IHttpContextAccessor httpContextAccessor, IMongoService mongoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _mongoService = mongoService;
    }

    public Task<List<GetUserDto>> Handle(GetAllChatUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var userId = ObjectId.Parse(_httpContextAccessor.HttpContext.User.Claims
            .First(e => e.Type == ClaimTypes.NameIdentifier).Value);

        var userChatGroups = _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId)).ToList();

        var userProjection = Builders<User>.Projection
            .Include(e => e.Id)
            .Include(e => e.UserName);

        var chatGroupUsers = _mongoService.GetCollection<User>()
            .Find(e => userChatGroups.SelectMany(e => e.UserIds).Contains(e.Id)).Project<GetUserDto>(userProjection)
            .ToList();

        return Task.FromResult(chatGroupUsers);
    }
}