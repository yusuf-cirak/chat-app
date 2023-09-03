namespace Application.Features.ChatGroups.Dtos;

public sealed class GetAllChatGroupDto
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    
    public GetAllChatGroupDto()
    {
        
    }
    public GetAllChatGroupDto(ObjectId id, string name)
    {
        Id = id;
        Name = name;
    }
}