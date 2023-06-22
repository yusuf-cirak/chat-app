namespace Domain.Entities;
public class Message : BaseEntity
{
    public ObjectId UserId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }


    public Message()
    {
        
    }

    public Message(ObjectId userId, string content, DateTime createdAt)
    {
        UserId = userId;
        Content = content;
        CreatedAt = createdAt;
    }
}