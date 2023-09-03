using Application.Features.Auths.Commands.Login;
using Application.Features.Auths.Commands.Refresh;
using Application.Features.Auths.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("_api/[controller]")]
[ApiController]
public sealed class ChatGroupsController : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommandRequest registerCommandRequest)
    {
        var response = await Mediator.Send(registerCommandRequest);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommandRequest loginCommandRequest)
    {
        var response = await Mediator.Send(loginCommandRequest);
        return Ok(response);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommandRequest refreshTokenCommandRequest)
    {
        var response = await Mediator.Send(refreshTokenCommandRequest);
        return Ok(response);
    }
}