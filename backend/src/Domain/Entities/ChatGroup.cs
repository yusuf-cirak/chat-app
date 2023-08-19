namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    public ChatGroup()
    {
        UserIds = new();
        Messages = new();
    }


    public ChatGroup(string? name, bool isPrivate) : this()
    {
        Name = name;
        IsPrivate = isPrivate;
    }

    [BsonIgnoreIfNull] public string? Name { get; set; }

    public bool IsPrivate { get; set; }

    public List<ObjectId> UserIds { get; set; }

    public List<Message> Messages { get; set; }
}