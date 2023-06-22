using Domain.Entities;

namespace Infrastructure.JWT;

public interface IJwtHelper
{
    AccessToken CreateToken(User user);
}