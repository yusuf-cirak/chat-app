using Application.Abstractions.Security;
using Application.Common.Extensions;
using Application.Features.ChatGroups.Rules;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Application.Features.ChatGroups.Commands.Create;

public readonly record struct CreateChatGroupCommandRequest
    (List<string> ParticipantUserIds, string Name, bool IsPrivate) : IRequest<string>, ISecuredRequest;

public sealed class CreateChatGroupCommandHandler : IRequestHandler<CreateChatGroupCommandRequest, string>
{
    private readonly IMongoService _mongoService;
    private readonly ChatGroupBusinessRules _chatGroupBusinessRules;
    private readonly ILogger<CreateChatGroupCommandRequest> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateChatGroupCommandHandler(IMongoService mongoService, ChatGroupBusinessRules chatGroupBusinessRules,
        ILogger<CreateChatGroupCommandRequest> logger, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _chatGroupBusinessRules = chatGroupBusinessRules;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(CreateChatGroupCommandRequest request, CancellationToken cancellationToken)
    {
        _chatGroupBusinessRules.UserMustExistInParticipantsBeforeCreatingChatGroup(request.ParticipantUserIds);
        // Chat group is private if it has only 2 participants and has no name
        if (request.IsPrivate)
        {
            // Scenario 1: Users already have a chat group together
            var chatGroupId = await FindExistingPrivateChatGroupIdAsync(request.ParticipantUserIds, cancellationToken);

            if (!string.IsNullOrEmpty(chatGroupId))
            {
                _logger.LogInformation("{RequestName} - Private chat group already exists for {UserId} {Username}",
                    nameof(CreateChatGroupCommandRequest),
                    _httpContextAccessor.HttpContext.User.GetUserId(),
                    _httpContextAccessor.HttpContext.User.GetUsername()
                );
                return chatGroupId;
            }

            // Scenario 2: Creating a new private chat group
            return await CreatePrivateChatGroupAsync(request.ParticipantUserIds, cancellationToken);
        }

        // Scenario 3: Creating a chat group with a specified name
        return await CreateNamedChatGroupAsync(request.Name, request.ParticipantUserIds, cancellationToken);
    }

    private async Task<string?> FindExistingPrivateChatGroupIdAsync(List<string> participantUserIds,
        CancellationToken cancellationToken)
    {
        var userChatGroupProjection = Builders<ChatGroup>.Projection.Expression(cg => cg.Id);

        var filter = Builders<ChatGroup>.Filter.Where(cg => cg.IsPrivate) &
                     Builders<ChatGroup>.Filter.All(u => u.UserIds, participantUserIds) &
                     Builders<ChatGroup>.Filter.Size(u => u.UserIds, participantUserIds.Count);


        var existingChatGroupId = await _mongoService.GetCollection<ChatGroup>()
            .Find(filter)
            .Project(userChatGroupProjection)
            .SingleOrDefaultAsync(cancellationToken);

        return existingChatGroupId;
    }

    private async Task<string> CreatePrivateChatGroupAsync(List<string> participantUserIds,
        CancellationToken cancellationToken)
    {
        var newChatGroup = new ChatGroup(participantUserIds, isPrivate: true);

        await _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);

        _logger.LogInformation("{RequestName} - {UserId} {Username} created a chat group",
            nameof(CreateChatGroupCommandRequest),
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername()
        );

        return newChatGroup.Id;
    }

    private async Task<string> CreateNamedChatGroupAsync(string groupName, List<string> participantUserIds,
        CancellationToken cancellationToken)
    {
        var newChatGroup = new ChatGroup(groupName, participantUserIds, isPrivate: false);

        await _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);

        _logger.LogInformation("{RequestName} - {UserId} {Username} created a chat group",
            nameof(CreateChatGroupCommandRequest),
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername());

        return newChatGroup.Id;
    }
}