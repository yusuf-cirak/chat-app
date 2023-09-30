namespace Domain.Entities;
public sealed class Message : BaseEntity
{
    public string UserId { get; set; }
    public string ChatGroupId { get; set; }
    public string Body { get; init; }
    public DateTime SentAt { get; set; }

    public Message()
    {
        
    }

    public Message(string userId,string chatGroupId, string body, DateTime sentAt)
    {
        UserId = userId;
        ChatGroupId = chatGroupId;
        Body = body;
        SentAt = sentAt;
    }
}