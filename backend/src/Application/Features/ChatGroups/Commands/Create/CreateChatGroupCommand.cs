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
            var chatGroupId = await FindExistingChatGroupIdAsync(request.ParticipantUserIds, cancellationToken);

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

    private async Task<ObjectId?> FindExistingChatGroupIdAsync(List<ObjectId> participantUserIds,
        CancellationToken cancellationToken)
    {
        var usersChatGroups = await _mongoService.GetCollection<UserChatGroup>()
            .Find(e => participantUserIds.Contains(e.UserId))
            .ToListAsync(cancellationToken: cancellationToken);

        var chatGroupId = usersChatGroups.GroupBy(e => e.ChatGroupId)
            .FirstOrDefault(e => e.Count() == 2)?.Key;

        return chatGroupId;
    }

    private async Task<ObjectId> CreatePrivateChatGroupAsync(List<ObjectId> participantUserIds,
        CancellationToken cancellationToken)
    {
        var newChatGroup = new ChatGroup();

        var userChatGroups = participantUserIds.Select(userId => new UserChatGroup(userId, newChatGroup.Id)).ToList();

        var createChatGroupTask = _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);
        var createUserChatGroupsTask = _mongoService.GetCollection<UserChatGroup>()
            .InsertManyAsync(userChatGroups, cancellationToken: cancellationToken);

        await Task.WhenAll(new Task[] { createChatGroupTask, createUserChatGroupsTask });

        return newChatGroup.Id;
    }

    private async Task<ObjectId> CreateNamedChatGroupAsync(string groupName, List<ObjectId> participantUserIds,
        CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        var newChatGroup = new ChatGroup(groupName);

        var createChatGroupTask = _mongoService.GetCollection<ChatGroup>()
            .InsertOneAsync(newChatGroup, cancellationToken: cancellationToken);

        tasks.Add(createChatGroupTask);

        var userChatGroupCollection = _mongoService.GetCollection<UserChatGroup>();

        var addUserTasks = participantUserIds.Select(userId =>
            userChatGroupCollection.InsertOneAsync(new UserChatGroup(userId, newChatGroup.Id),
                cancellationToken: cancellationToken));

        tasks.AddRange(addUserTasks);

        await Task.WhenAll(tasks);

        return newChatGroup.Id;
    }
}