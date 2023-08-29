namespace Domain.Entities;

public sealed class PrivateChatGroup : BaseAuditableEntity
{
    public List<ObjectId> UserIds { get; set; }
    
    public PrivateChatGroup()
    {
        UserIds = new(2);
    }
}