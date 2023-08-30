namespace Domain.Common;
public abstract class BaseEntity
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

}