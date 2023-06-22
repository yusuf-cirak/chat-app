namespace Infrastructure.JWT;

public record AccessToken(string Token, DateTime Expiration);