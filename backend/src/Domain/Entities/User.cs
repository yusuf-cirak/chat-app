namespace Domain.Entities;

public sealed class User : BaseEntity
{
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;

    public User()
    {
    }
    
    public User(string userName, byte[] passwordHash, byte[] passwordSalt)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
    
    public User(string userName, byte[] passwordHash, byte[] passwordSalt,string profileImageUrl)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        ProfileImageUrl = profileImageUrl;
    }
}