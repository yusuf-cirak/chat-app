namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    [BsonIgnoreIfDefault] public string Name { get; set; }

    public List<ObjectId> UserIds { get; set; }


    public ChatGroup()
    {
        UserIds = new();
    }
    
    public ChatGroup(string name, List<ObjectId> userIds)
    {
        Name = name;
        UserIds = userIds;
    }
    
    public ChatGroup(List<ObjectId> userIds)
    {
        UserIds = userIds;
    }
}