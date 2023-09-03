namespace Domain.Entities;
public sealed class Message : BaseEntity
{
    public ObjectId UserId { get; set; }
    public ObjectId ChatGroupId { get; set; }
    public string Body { get; init; }
    public DateTime SentAt { get; set; }

    public Message()
    {
        
    }

    public Message(ObjectId userId,ObjectId chatGroupId, string body, DateTime sentAt)
    {
        UserId = userId;
        ChatGroupId = chatGroupId;
        Body = body;
        SentAt = sentAt;
    }
}