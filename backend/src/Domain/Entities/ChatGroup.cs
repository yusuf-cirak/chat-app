namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    [BsonIgnoreIfDefault]
    public string Name { get; set; }
    
    public ChatGroup()
    {
        
    }

    public ChatGroup(string name)
    {
        Name = name;
    }

}