namespace Application.Features.ChatGroups.Dtos;

public sealed class GetChatGroupDto
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    
    public GetChatGroupDto()
    {
        
    }
    public GetChatGroupDto(ObjectId id, string name)
    {
        Id = id;
        Name = name;
    }
}