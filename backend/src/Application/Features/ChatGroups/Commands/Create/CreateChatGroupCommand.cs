using Application.Abstractions.Security;
using Application.Features.ChatGroups.Rules;
using MongoDB.Driver;

namespace Application.Features.ChatGroups.Commands.Create;

public readonly record struct CreateChatGroupCommandRequest
    (List<ObjectId> ParticipantUserIds, string Name = default!) : IRequest<ObjectId>, ISecuredRequest;

public sealed class CreateChatGroupCommandHandler : IRequestHandler<CreateChatGroupCommandRequest, ObjectId>
{
    private readonly IMongoService _mongoService;
    private readonly ChatGroupBusinessRules _chatGroupBusinessRules;

    public CreateChatGroupCommandHandler(IMongoService mongoService, ChatGroupBusinessRules chatGroupBusinessRules)
    {
        _mongoService = mongoService;
        _chatGroupBusinessRules = chatGroupBusinessRules;
    }

    public async Task<ObjectId> Handle(CreateChatGroupCommandRequest request, CancellationToken cancellationToken)
    {
        _chatGroupBusinessRules.UserMustExistInParticipantsBeforeCreatingChatGroup(request.ParticipantUserIds);
        // Chat group is private if it has only 2 participants and has no name
        if (request.Name == default!)
        {
            // Scenario 1: Users already have a chat group together
            var chatGroupId = FindExistingChatGroupIdAsync(request.ParticipantUserIds, cancellationToken);

            if (chatGroupId.HasValue)
            {
                return chatGroupId.Value;
            }

            // Scenario 2: Creating a new private chat group
            return await CreatePrivateChatGroupAsync(request.ParticipantUserIds, cancellationToken);
        }

        // Scenario 3: Creating a chat group with a specified name
        return await CreateNamedChatGroupAsync(request.Name!, request.ParticipantUserIds, cancellationToken);
    }

    private ObjectId? FindExistingChatGroupIdAsync(List<ObjectId> participantUserIds,
        CancellationToken cancellationToken)
    {
        var userChatGroupProjection = Builders<ChatGroup>.Projection
            .Include(e => e.Id);
        var chatGroupId = _mongoService.GetCollection<ChatGroup>()
            .Find(cg => cg.UserIds.Count == 2 && cg.UserIds.All(participantUserIds.Contains))
            .Project<ObjectId?>(userChatGroupProjection).First();

        return chatGroupId;
    }

    private async Task<ObjectId> CreatePrivateChatGroupAsync(List<ObjectId> participantUserIds,
        CancellationToken cancellationToken)
    {
        var newChatGroup = new ChatGroup(participantUserIds);

        await _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);

        return newChatGroup.Id;
    }

    private async Task<ObjectId> CreateNamedChatGroupAsync(string groupName, List<ObjectId> participantUserIds,
        CancellationToken cancellationToken)
    {
        var newChatGroup = new ChatGroup(groupName, participantUserIds);

        await _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);
        
        return newChatGroup.Id;
    }
}