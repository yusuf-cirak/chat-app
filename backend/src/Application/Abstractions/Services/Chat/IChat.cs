using MongoDB.Bson;

namespace Application.Abstractions.Services.Chat;

public interface IChat
{
    bool AddUser(string userId, string connectionId);
    bool RemoveUser(string userId);
    string GetUserId(string connectionId);
    string GetConnectionId(string userId);
    List<string> GetOnlineUsers();
    
}

