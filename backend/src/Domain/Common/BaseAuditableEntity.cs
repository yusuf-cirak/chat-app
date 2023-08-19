namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public ObjectId CreatedBy { get; set; }
    [BsonIgnoreIfDefault]
    public DateTime LastModified { get; set; }
    [BsonIgnoreIfNull]
    public ObjectId? LastModifiedBy { get; set; }
}