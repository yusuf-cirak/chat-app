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
    
    public bool AddUser(string userId,string connectionId)
    {
        return _chatService.AddUser(userId, connectionId);
    }
    public bool RemoveUser(string userId)
    {
        return _chatService.RemoveUser(userId);
    }

    public string GetUserId(string connectionId)
    {
        return _chatService.GetUserId(connectionId);
    }

    public string GetConnectionId(string userId)
    {
        return _chatService.GetConnectionId(userId);
    }

    public List<string> GetOnlineUsers()
    {
        return _chatService.GetOnlineUsers();
    }
}