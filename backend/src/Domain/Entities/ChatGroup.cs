namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    [BsonIgnoreIfDefault] public string Name { get; set; }

    public List<ObjectId> UserIds { get; set; }

    public bool IsPrivate { get; set; }


    public ChatGroup()
    {
        UserIds = new();
    }
    
    public ChatGroup(string name, List<ObjectId> userIds,bool isPrivate)
    {
        Name = name;
        UserIds = userIds;
        IsPrivate = isPrivate;
    }
    
    public ChatGroup(List<ObjectId> userIds,bool isPrivate)
    {
        UserIds = userIds;
        IsPrivate = isPrivate;
    }
}