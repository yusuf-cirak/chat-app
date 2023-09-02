namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    public string Name { get; set; }
    
    public ChatGroup(string? name)
    {
        Name = name;
    }

}