namespace Application.Features.Users.Dtos;

public readonly record struct GetUserDto(ObjectId Id, string UserName);