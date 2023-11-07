using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Common.Extensions;
using Application.Features.ChatGroups.Dtos;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Queries.GetAll;

public readonly record struct GetAllChatGroupsQueryRequest : IRequest<List<GetChatGroupDto>>, ISecuredRequest;

public sealed class GetAllChatGroupsQueryHandler : IRequestHandler<GetAllChatGroupsQueryRequest, List<GetChatGroupDto>>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetAllChatGroupsQueryRequest> _logger;

    public GetAllChatGroupsQueryHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor,
        ILogger<GetAllChatGroupsQueryRequest> logger)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<List<GetChatGroupDto>> Handle(GetAllChatGroupsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier)
            .Value);

        var chatGroupProjection = Builders<ChatGroup>.Projection
            .Include(e => e.Id)
            .Include(e => e.Name)
            .Include(c => c.IsPrivate)
            .Include(c => c.UserIds)
            .Include(c => c.ProfileImageUrl);

        var chatGroupsDto = await _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId!))
            .Project<GetChatGroupDto>(chatGroupProjection).ToListAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("{RequestName} - {ChatGroupCount} of groups retrieved for {UserId} - {Username}",
            nameof(GetAllChatGroupsQueryRequest),
            chatGroupsDto.Count,
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername()
        );

        return chatGroupsDto;
    }
}