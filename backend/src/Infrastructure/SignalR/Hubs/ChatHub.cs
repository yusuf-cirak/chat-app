using Application.Abstractions.Services.Chat;
using Application.Features.ChatGroups.Dtos;
using Infrastructure.Dtos.Hub;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR.Hubs;

public sealed class ChatHub : Hub<IChatHub>
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        _chatService.AddUser(Context.User.GetUserId(), Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _chatService.RemoveUser(Context.User.GetUserId());
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageAsync(MessageDto messageDto,List<string> recipientUserIds)
    {
        List<string> recipientUserConnectionIds = _chatService.GetConnectionIds(recipientUserIds);

        var receiveMessageTasks = recipientUserConnectionIds.Select(recipientUserConnectionId =>
            Clients.Client(recipientUserConnectionId).ReceiveMessageAsync(messageDto));

        await Task.WhenAll(receiveMessageTasks);
    }
    
    public async Task CreateChatGroupAsync(GetChatGroupDto chatGroup)
    {
        var createChatGroupTasks = chatGroup.UserIds.Select(userId =>
            Clients.Client(_chatService.GetConnectionId(userId)).ChatGroupCreatedAsync(chatGroup));
        
        await Task.WhenAll(createChatGroupTasks);
    }
    
}