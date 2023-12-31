﻿using System.Security.Claims;
using Application.Abstractions.Security;
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
        var userId = (_httpContextAccessor.HttpContext.User.Claims
            .First(e => e.Type == ClaimTypes.NameIdentifier).Value);

        var chatGroupProjection = Builders<ChatGroup>.Projection
            .Expression(cg => cg.UserIds);
        
        var userIds = _mongoService.GetCollection<ChatGroup>().Find(e => e.UserIds.Contains(userId)).Project(chatGroupProjection).ToList().SelectMany(cg=>cg).ToList();

        var userProjection = Builders<User>.Projection
            .Include(e => e.Id)
            .Include(e => e.UserName)
            .Include(u => u.ProfileImageUrl);

        var chatGroupUsers = _mongoService.GetCollection<User>()
            .Find(e => userIds.Contains(e.Id)).Project<GetUserDto>(userProjection)
            .ToList();

        return Task.FromResult(chatGroupUsers);
    }
}