using Application.Features.Users.Dtos;

namespace Infrastructure.Dtos.Hub;

public sealed class GetHubChatGroupDto
{
    public string Id { get; set; }
    public string Name { get; set; }

    public List<GetUserDto> Users { get; set; }
    public bool IsPrivate { get; set; }
    public string ProfileImageUrl { get; set; }
    
    public GetHubChatGroupDto()
    {
        
    }
    public GetHubChatGroupDto(string id, string name, bool isPrivate,List<GetUserDto> users)
    {
        Id = id;
        Name = name;
        IsPrivate = isPrivate;
        Users = users;
    }
    
    public GetHubChatGroupDto(string id, string name, bool isPrivate,List<GetUserDto> users,string profileImageUrl)
    {
        Id = id;
        Name = name;
        IsPrivate = isPrivate;
        Users = users;
        ProfileImageUrl = profileImageUrl;
    }
}