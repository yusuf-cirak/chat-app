namespace Domain.Common;
public abstract class BaseEntity
{
    // [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString()!;

}