namespace Application.Features.Messages.Dtos;

public sealed class GetAllMessagesDto
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<Message> Messages { get; set; }
}