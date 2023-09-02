using Application.Abstractions.Security;
using Application.Abstractions.Services;
using Application.Features.PrivateChatGroups.Rules;
using MongoDB.Bson;

namespace Application.Features.PrivateChatGroups.Commands.Create;

public readonly record struct CreatePrivateChatGroupCommandRequest(ObjectId UserId, ObjectId ParticipantUserId): IRequest<ObjectId>,ISecuredRequest;


public sealed class CreatePrivateChatGroupCommandHandler : IRequestHandler<CreatePrivateChatGroupCommandRequest,ObjectId>
{
    private readonly IMongoService _mongoService;
    private readonly PrivateChatGroupBusinessRules _privateChatGroupBusinessRules;

    public CreatePrivateChatGroupCommandHandler(IHttpContextAccessor httpContextAccessor, IMongoService mongoService, PrivateChatGroupBusinessRules privateChatGroupBusinessRules)
    {
        _mongoService = mongoService;
        _privateChatGroupBusinessRules = privateChatGroupBusinessRules;
    }

    public async Task<ObjectId> Handle(CreatePrivateChatGroupCommandRequest request, CancellationToken cancellationToken)
    {
        _privateChatGroupBusinessRules.VerifyUserIdBeforeCreatingPrivateChatGroup(request.UserId.ToString()!);

        // Creating a new private chat group
        var newPrivateChatGroup = new PrivateChatGroup();
        
        // Adding users to chat group with the chat group id
        List<UserChatGroup> userChatGroups = new List<UserChatGroup>
        {
            new UserChatGroup
            {
                ChatGroupId = newPrivateChatGroup.Id,
                UserId = request.UserId
            },
            new UserChatGroup
            {
                ChatGroupId = newPrivateChatGroup.Id,
                UserId = request.ParticipantUserId
            }
        };
        
        
        var createChatGroupTask = _mongoService.GetCollection<PrivateChatGroup>().InsertOneAsync(newPrivateChatGroup, cancellationToken: cancellationToken);
       
        var createUserChatGroupsTask = _mongoService.GetCollection<UserChatGroup>().InsertManyAsync(userChatGroups, cancellationToken: cancellationToken);
        
        // Creating tasks array to avoid unnecessary allocation on WhenAll
        Task[] tasks = new []{createChatGroupTask, createUserChatGroupsTask};

        // Waiting for both tasks to complete
        await Task.WhenAll(tasks);

        return newPrivateChatGroup.Id;
    }
}
