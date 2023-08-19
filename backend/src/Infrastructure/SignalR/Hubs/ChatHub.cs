using Application.Abstractions.Services.Chat;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace Infrastructure.SignalR.Hubs;

public sealed class ChatHub : Hub
{
    private readonly IChat _chat;

    public ChatHub(IChat chat)
    {
        _chat = chat;
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRConstants.ChatUsers);
        _chat.AddUser(ObjectId.Parse(Context.UserIdentifier));
        _chat.AddUserConnectionId(ObjectId.Parse(Context.UserIdentifier), Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, SignalRConstants.ChatUsers);
        await base.OnDisconnectedAsync(exception);
    }
}