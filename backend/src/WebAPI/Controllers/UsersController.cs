using Application.Features.Auths.Commands.Login;
using Application.Features.Auths.Commands.Refresh;
using Application.Features.Auths.Commands.Register;
using Application.Features.Users.Commands.RemoveImage;
using Application.Features.Users.Commands.UploadImage;
using Application.Features.Users.Queries.GetAllChatUsers;
using Application.Features.Users.Queries.GetByName;
using Application.Features.Users.Queries.GetOnlineChatUsers;
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

    [HttpGet("online-users")]
    public async Task<IActionResult> GetOnlineChatUsers()
    {
        var response = await Mediator.Send(new GetOnlineChatUsersQueryRequest());
        return Ok(response);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchUsersByName([FromQuery] GetUsersByNameQueryRequest getUsersByNameQueryRequest)
    {
        var response = await Mediator.Send(getUsersByNameQueryRequest);
        return Ok(response);
    }
    
    [HttpPost("profile-image")]
    public async Task<IActionResult> UploadProfileImage([FromForm] UploadImageCommandRequest uploadImageCommandRequest)
    {
        var response = await Mediator.Send(uploadImageCommandRequest);
        return Ok(response);
    }
    
    [HttpDelete("profile-image")]
    public async Task<IActionResult> RemoveProfileImage(RemoveImageCommandRequest removeImageCommandRequest)
    {
        var response = await Mediator.Send(removeImageCommandRequest);
        return Ok(response);
    }
}