using Application.Abstractions.Security;
using Application.Features.ChatGroups.Rules;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Application.Features.ChatGroups.Commands.Create;

public readonly record struct CreateChatGroupCommandRequest
    (List<string> ParticipantUserIds, string Name,bool IsPrivate) : IRequest<string>, ISecuredRequest;

public sealed class CreateChatGroupCommandHandler : IRequestHandler<CreateChatGroupCommandRequest, string>
{
    private readonly IMongoService _mongoService;
    private readonly ChatGroupBusinessRules _chatGroupBusinessRules;

    public CreateChatGroupCommandHandler(IMongoService mongoService, ChatGroupBusinessRules chatGroupBusinessRules)
    {
        _mongoService = mongoService;
        _chatGroupBusinessRules = chatGroupBusinessRules;
    }

    public async Task<string> Handle(CreateChatGroupCommandRequest request, CancellationToken cancellationToken)
    {
        _chatGroupBusinessRules.UserMustExistInParticipantsBeforeCreatingChatGroup(request.ParticipantUserIds);
        // Chat group is private if it has only 2 participants and has no name
        if (request.IsPrivate)
        {
            // Scenario 1: Users already have a chat group together
            var chatGroupId = await FindExistingChatGroupIdAsync(request.ParticipantUserIds, cancellationToken);

            if (!string.IsNullOrEmpty(chatGroupId))
            {
                return chatGroupId;
            }

            // Scenario 2: Creating a new private chat group
            return await CreatePrivateChatGroupAsync(request.ParticipantUserIds, cancellationToken);
        }

        // Scenario 3: Creating a chat group with a specified name
        return await CreateNamedChatGroupAsync(request.Name, request.ParticipantUserIds, cancellationToken);
    }

    private async Task<string?> FindExistingChatGroupIdAsync(List<string> participantUserIds,
        CancellationToken cancellationToken)
    {
        var userChatGroupProjection = Builders<ChatGroup>.Projection.Expression(cg => cg.Id);
        
        var filter = Builders<ChatGroup>.Filter.Where(chatGroup=> chatGroup.IsPrivate && chatGroup.UserIds.All(id=>participantUserIds.Contains(id)));

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

        return newChatGroup.Id;
    }

    private async Task<string> CreateNamedChatGroupAsync(string groupName, List<string> participantUserIds,
        CancellationToken cancellationToken)
    {
        var newChatGroup = new ChatGroup(groupName, participantUserIds, isPrivate: false);

        await _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);
        
        return newChatGroup.Id;
    }
}