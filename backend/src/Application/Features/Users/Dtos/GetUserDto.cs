using MongoDB.Bson.Serialization.Attributes;

namespace Application.Features.Users.Dtos;

public sealed class GetUserDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string ProfileImageUrl { get; set; }

    public GetUserDto()
    {
        
    }
    
    public GetUserDto(string id, string userName)
    {
        Id = id;
        UserName = userName;
    }
}