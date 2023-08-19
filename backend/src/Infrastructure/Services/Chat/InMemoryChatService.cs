using Application.Abstractions.Services.Chat;
using MongoDB.Bson;

namespace Infrastructure.Services.Chat;

public sealed class InMemoryChatService : IChatService
{
    private readonly Dictionary<ObjectId, string> _users = new();

    public bool AddUser(ObjectId userId)
    {
        lock (_users)
        {
            foreach (var user in _users)
            {
                if (user.Key == userId)
                {
                    return false;
                }
            }

            _users.Add(userId, "");
            return true;
        }
    }

    public bool AddUserConnectionId(ObjectId userId, string connectionId)
    {
        lock (_users)
        {
            if (!_users.ContainsKey(userId))
            {
                return false;
            }

            _users[userId] = connectionId;
            return true;
        }
    }

    public bool RemoveUser(ObjectId userId)
    {
        lock (_users)
        {
            return _users.Remove(userId);
        }
    }


    public ObjectId GetUserId(string connectionId)
    {
        lock (_users)
        {
            return _users.Where(u => u.Value == connectionId).Select(u => u.Key).FirstOrDefault();
        }
    }

    public string GetConnectionId(ObjectId userId)
    {
        lock (_users)
        {
            return _users.Where(u => u.Key == userId).Select(u => u.Value).FirstOrDefault() ?? "";
        }
    }

    public List<ObjectId> GetOnlineUsers()
    {
        lock (_users)
        {
            return _users.Select(u => u.Key).ToList();
        }
    }
}   