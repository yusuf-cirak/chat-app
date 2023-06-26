namespace Domain.Entities;

public class ChatGroup : BaseAuditableEntity
{
    public ChatGroup()
    {
    }


    public ChatGroup(string? name, bool isPrivate)
    {
        Name = name;
        IsPrivate = isPrivate;
    }

    [BsonIgnoreIfNull] public string? Name { get; set; }

    public bool IsPrivate { get; set; }

    public virtual List<ObjectId> UserIds { get; set; }

    public virtual List<Message> Messages { get; set; }
}