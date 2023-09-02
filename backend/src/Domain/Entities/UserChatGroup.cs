namespace Domain.Entities;

public sealed class UserChatGroup : BaseEntity
{
    public ObjectId UserId { get; set; }
    public ObjectId ChatGroupId { get; set; }
    
    public UserChatGroup()
    {
        
    }
    
    public UserChatGroup(ObjectId userId, ObjectId chatGroupId)
    {
        UserId = userId;
        ChatGroupId = chatGroupId;
    }
}