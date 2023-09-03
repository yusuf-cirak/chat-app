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
    private readonly IChat _chat;

    public ChatHub(IChat chat)
    {
        _chat = chat;
    }

    public override async Task OnConnectedAsync()
    {
        _chat.AddUser(Context.User.GetUserId(), Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _chat.RemoveUser(Context.User.GetUserId());
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageAsync(string recipientUserId, string chatGroupId, string message)
    {
        string senderUserId = Context.User.GetUserId();

        string recipientUserConnectionId = _chat.GetConnectionId(recipientUserId);

        if (!string.IsNullOrEmpty(recipientUserConnectionId))
        {
            await Clients.Client(recipientUserConnectionId).ReceiveMessageAsync(senderUserId, message);
        }
    }
}