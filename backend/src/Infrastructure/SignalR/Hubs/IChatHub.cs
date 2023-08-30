namespace Infrastructure.SignalR.Hubs;

public interface IChatHub
{
    Task SendMessageAsync(string recipientUserId,string message);
    Task ReceiveMessageAsync(string senderUserId,string message);
}