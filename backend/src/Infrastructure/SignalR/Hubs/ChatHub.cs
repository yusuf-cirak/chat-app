using Application.Abstractions.Services.Chat;
using Infrastructure.Extensions;
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

    public async Task SendMessage(string message)
    {
        // await Clients.All.ReceiveMessage(Context.User.GetUserId(), message);
    }
}