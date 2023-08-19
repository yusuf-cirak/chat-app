using Application.Common.Models;
using Domain.Entities;

namespace Application.Abstractions.Helpers;

public interface IJwtHelper
{
    AccessToken CreateToken(User user);
}