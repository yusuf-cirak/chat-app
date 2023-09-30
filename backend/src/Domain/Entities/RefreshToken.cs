namespace Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string UserId { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Token { get; set; }

    public RefreshToken()
    {
        
    }

    public RefreshToken(string token,string userId,string createdByIp, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        CreatedByIp = createdByIp;
        ExpiresAt = expiresAt;
    }
}