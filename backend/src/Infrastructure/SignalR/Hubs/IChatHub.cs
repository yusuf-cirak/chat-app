using Application.Features.ChatGroups.Dtos;
using Infrastructure.Dtos.Hub;

namespace Infrastructure.SignalR.Hubs;

public interface IChatHub
{
    Task SendMessageAsync(MessageDto messageDto);
    Task ReceiveMessageAsync(MessageDto messageDto);
    Task ChatGroupCreatedAsync(GetChatGroupDto chatGroup);
}