using Application.Features.Auths.Commands.Login;
using Application.Features.Auths.Commands.Refresh;
using Application.Features.Auths.Commands.Register;
using Application.Features.ChatGroups.Commands.Create;
using Application.Features.ChatGroups.Commands.UploadImage;
using Application.Features.ChatGroups.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("_api/[controller]")]
[ApiController]
public sealed class ChatGroupsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await Mediator.Send(new GetAllChatGroupsQueryRequest());
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateChatGroupCommandRequest createChatGroupCommandRequest)
    {
        var response = await Mediator.Send(createChatGroupCommandRequest);
        return Ok(response);
    }
    
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage([FromForm] UploadChatGroupImageCommandRequest uploadChatGroupImageCommandRequest)
    {
        var response = await Mediator.Send(uploadChatGroupImageCommandRequest);
        return Ok(response);
    }
}