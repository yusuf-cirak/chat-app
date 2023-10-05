namespace Application.Abstractions.Services.Chat;

public interface IChatService
{
    bool AddUser(string userId, string connectionId);
    bool RemoveUser(string userId);
    string GetUserId(string connectionId);
    List<string> GetConnectionIds(List<string> userIds);
    string GetConnectionId(string userId);
    List<string> GetOnlineUsers();
}