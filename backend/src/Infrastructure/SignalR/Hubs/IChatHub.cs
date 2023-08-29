namespace Infrastructure.SignalR.Hubs;

public interface IChatHub
{
    Task SendMessageAsync(string message);
}