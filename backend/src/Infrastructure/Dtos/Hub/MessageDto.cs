namespace Infrastructure.Dtos.Hub;

// MessageId,SentById,SentAt,ChatGroupOrUserId,
public readonly record struct MessageDto(string Id, string UserId, string ChatGroupId, string Body, DateTime SentAt);