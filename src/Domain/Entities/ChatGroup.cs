namespace Domain.Entities;

public class ChatRoom : BaseAuditableEntity
{
    [BsonIgnoreIfNull]
    public string? Name { get; set; }
    public bool IsPrivate { get; set; }
    
    public virtual List<ObjectId> UserIds { get; set; }

    public virtual List<Message> Messages { get; set; }

    public ChatRoom()
    {
    }

    public ChatRoom(string? name, bool isPrivate)
    {
        Name = name;
        IsPrivate = isPrivate;
    }
}