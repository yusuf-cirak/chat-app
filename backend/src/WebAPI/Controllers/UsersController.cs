using Application.Features.Auths.Commands.Login;
using Application.Features.Auths.Commands.Refresh;
using Application.Features.Auths.Commands.Register;
using Application.Features.Users.Queries.GetAllChatUsers;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("_api/[controller]")]
[ApiController]
public sealed class UsersController : BaseController
{
    [HttpGet("chat-users")]
    public async Task<IActionResult> GetChatUsers()
    {
        var response = await Mediator.Send(new GetAllChatUsersQueryRequest());
        return Ok(response);
    }
}