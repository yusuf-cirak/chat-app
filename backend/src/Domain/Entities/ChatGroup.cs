namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    public string? Name { get; set; }
    
    public List<ObjectId> UserIds { get; set; }
    
    public ChatGroup()
    {
        UserIds = new();
    }

    public ChatGroup(string? name) : this()
    {
        Name = name;
    }

}