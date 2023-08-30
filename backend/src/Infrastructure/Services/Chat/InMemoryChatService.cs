using Application.Abstractions.Services.Chat;

namespace Infrastructure.Services.Chat;

public sealed class InMemoryChatService : IChatService
{
    private readonly Dictionary<string, string> _users = new();

    public bool AddUser(string userId,string connectionId)
    {
        lock (_users)
        {
            return _users.TryAdd(userId, connectionId);
        }
    }

    public bool AddUserConnectionId(string userId, string connectionId)
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

    public bool RemoveUser(string userId)
    {
        lock (_users)
        {
            return _users.Remove(userId);
        }
    }


    public string GetUserId(string connectionId)
    {
        lock (_users)
        {
            return _users.Where(u => u.Value == connectionId).Select(u => u.Value).SingleOrDefault()!;
        }
    }

    public string GetConnectionId(string userId)
    {
        lock (_users)
        {
            return _users.Where(u => u.Key == userId).Select(u => u.Value).SingleOrDefault()!;
        }
    }

    public List<string> GetOnlineUsers()
    {
        lock (_users)
        {
            return _users.Select(u => u.Key).ToList();
        }
    }
}   