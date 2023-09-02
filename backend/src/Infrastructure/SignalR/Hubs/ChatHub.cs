using Application.Abstractions.Services;
using Application.Abstractions.Services.Chat;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace Infrastructure.SignalR.Hubs;

public sealed class ChatHub : Hub<IChatHub>
{
    private readonly IChat _chat;
    private readonly IMongoService _mongoService;

    public ChatHub(IChat chat, IMongoService mongoService)
    {
        _chat = chat;
        _mongoService = mongoService;
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

        Message messageObj = new(userId: ObjectId.Parse(senderUserId), chatGroupId: ObjectId.Parse(chatGroupId),
            body: message, sentAt: DateTime.Now);

        await _mongoService.GetCollection<Message>().InsertOneAsync(messageObj);

        string recipientUserConnectionId = _chat.GetConnectionId(recipientUserId);
        
        await Clients.Client(recipientUserConnectionId).ReceiveMessageAsync(senderUserId, message);
    }
}