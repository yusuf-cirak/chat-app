
using Application.Features.PrivateChatGroups.Commands.Create;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("_api/[controller]")]
[ApiController]
public sealed class PrivateChatGroupsController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Register(CreatePrivateChatGroupCommandRequest createPrivateChatGroupCommandRequest)
    {
        var response = await Mediator.Send(createPrivateChatGroupCommandRequest);
        return Ok(response);
    }
}