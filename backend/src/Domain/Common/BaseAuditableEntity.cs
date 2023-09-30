namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    [BsonIgnoreIfDefault]
    public DateTime CreatedAt { get; set; }

    [BsonIgnoreIfDefault]
    public string CreatedBy { get; set; }

    [BsonIgnoreIfDefault]
    public DateTime LastModified { get; set; }

    [BsonIgnoreIfDefault]
    public string? LastModifiedBy { get; set; }
}