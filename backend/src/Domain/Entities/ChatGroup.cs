﻿namespace Domain.Entities;

public sealed class ChatGroup : BaseAuditableEntity
{
    [BsonIgnoreIfDefault] public string Name { get; set; }


    public List<string> UserIds { get; set; }

    public bool IsPrivate { get; set; }


    public ChatGroup()
    {
        UserIds = new();
    }
    
    public ChatGroup(string name, List<string> userIds,bool isPrivate)
    {
        Name = name;
        UserIds = userIds;
        IsPrivate = isPrivate;
    }
    
    public ChatGroup(List<string> userIds,bool isPrivate)
    {
        UserIds = userIds;
        IsPrivate = isPrivate;
    }
}