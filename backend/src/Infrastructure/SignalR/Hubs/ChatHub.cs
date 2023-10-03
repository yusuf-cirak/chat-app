using Application.Abstractions.Services;
using Application.Abstractions.Services.Chat;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

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

    public async Task SendMessageAsync(string recipientUserId, string chatGroupId, string message)
    {
        string senderUserId = Context.User.GetUserId();

        string recipientUserConnectionId = _chatService.GetConnectionId(recipientUserId);

        if (!string.IsNullOrEmpty(recipientUserConnectionId))
        {
            await Clients.Client(recipientUserConnectionId).ReceiveMessageAsync(senderUserId, message);
        }
    }
}