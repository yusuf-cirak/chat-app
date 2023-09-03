using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Features.ChatGroups.Dtos;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Queries.GetAll;

public readonly record struct GetAllChatGroupsQueryRequest : IRequest<List<GetAllChatGroupDto>>,ISecuredRequest;

public sealed class GetAllChatGroupsQueryHandler : IRequestHandler<GetAllChatGroupsQueryRequest, List<GetAllChatGroupDto>>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllChatGroupsQueryHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<GetAllChatGroupDto>> Handle(GetAllChatGroupsQueryRequest request, CancellationToken cancellationToken)
    {
        var userId = ObjectId.Parse(_httpContextAccessor.HttpContext?.User.Claims.First(e=>e.Type==ClaimTypes.NameIdentifier).Value);

        var userChatGroupProjection = Builders<UserChatGroup>.Projection
            .Include(e => e.Id);
        var userChatGroups = _mongoService.GetCollection<UserChatGroup>().Find(e => e.UserId == userId).Project<ObjectId>(userChatGroupProjection)
            .ToList();
        
        var chatGroups = await _mongoService.GetCollection<ChatGroup>().Find(e => userChatGroups.Contains(e.Id)).ToListAsync(cancellationToken: cancellationToken);
        
        var chatGroupDtos = chatGroups.Select(chatGroup => new GetAllChatGroupDto(chatGroup.Id,chatGroup.Name)).ToList();

        return chatGroupDtos;
    }
}