namespace Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public ObjectId UserId { get; set; }
    public string TokenHash { get; set; }
    public string TokenSalt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public RefreshToken()
    {
        
    }

    public RefreshToken(string tokenHash, string tokenSalt, DateTime expiresAt)
    {
        TokenHash = tokenHash;
        TokenSalt = tokenSalt;
        ExpiresAt = expiresAt;
    }
}