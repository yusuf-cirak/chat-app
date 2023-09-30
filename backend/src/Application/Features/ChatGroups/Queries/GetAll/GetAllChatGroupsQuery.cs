using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Features.ChatGroups.Dtos;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Queries.GetAll;

public readonly record struct GetAllChatGroupsQueryRequest : IRequest<List<GetChatGroupDto>>,ISecuredRequest;

public sealed class GetAllChatGroupsQueryHandler : IRequestHandler<GetAllChatGroupsQueryRequest, List<GetChatGroupDto>>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllChatGroupsQueryHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<GetChatGroupDto>> Handle(GetAllChatGroupsQueryRequest request, CancellationToken cancellationToken)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.Claims.First(e=>e.Type==ClaimTypes.NameIdentifier).Value);

        var chatGroupProjection = Builders<ChatGroup>.Projection
            .Include(e => e.Id)
            .Include(e => e.Name)
            .Include(c => c.IsPrivate)
            .Include(c => c.UserIds);
        
        var chatGroupsDto = await _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId!)).Project<GetChatGroupDto>(chatGroupProjection).ToListAsync(cancellationToken: cancellationToken);

        return chatGroupsDto;
    }
}