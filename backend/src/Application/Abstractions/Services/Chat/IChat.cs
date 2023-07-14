using MongoDB.Bson;

namespace Application.Abstractions.Services.Chat;

public interface IChat
{
    bool AddUser(ObjectId userId);
    bool AddUserConnectionId(ObjectId userId, string connectionId);
    bool RemoveUser(ObjectId userId);
    ObjectId GetUserId(string connectionId);
    string GetConnectionId(ObjectId userId);
    List<ObjectId> GetOnlineUsers();
    
}

