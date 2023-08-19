using Application.Abstractions.Services.Chat;
using MongoDB.Bson;

namespace Infrastructure.Services.Chat;

public sealed class Chat : IChat
{
    private readonly IChatService _chatService;

    public Chat(IChatService chatService)
    {
        _chatService = chatService;
    }
    
    public bool AddUser(ObjectId userId)
    {
        return _chatService.AddUser(userId);
    }
    public bool AddUserConnectionId(ObjectId userId, string connectionId)
    {
        return _chatService.AddUserConnectionId(userId, connectionId);
    }

    public bool RemoveUser(ObjectId userId)
    {
        return _chatService.RemoveUser(userId);
    }

    public ObjectId GetUserId(string connectionId)
    {
        return _chatService.GetUserId(connectionId);
    }

    public string GetConnectionId(ObjectId userId)
    {
        return _chatService.GetConnectionId(userId);
    }

    public List<ObjectId> GetOnlineUsers()
    {
        return _chatService.GetOnlineUsers();
    }
}