using Application.Common.Models;
using Domain.Entities;

namespace Application.Services.Abstractions;

public interface IJwtHelper
{
    AccessToken CreateToken(User user);
}