namespace Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public ObjectId UserId { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Token { get; set; }

    public RefreshToken()
    {
        
    }

    public RefreshToken(string token,ObjectId userId,string createdByIp, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        CreatedByIp = createdByIp;
        ExpiresAt = expiresAt;
    }
}