using MongoDB.Bson.Serialization.Attributes;

namespace Application.Features.ChatGroups.Dtos;

public sealed class GetChatGroupDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }

    public List<string> UserIds { get; set; }
    public bool IsPrivate { get; set; }
    
    public GetChatGroupDto()
    {
        
    }
    public GetChatGroupDto(string id, string name, bool isPrivate,List<string> userIds)
    {
        Id = id;
        Name = name;
        IsPrivate = isPrivate;
        UserIds = userIds;
    }
}