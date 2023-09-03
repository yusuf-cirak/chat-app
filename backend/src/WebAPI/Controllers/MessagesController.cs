using Application.Features.ChatGroups.Commands.Create;
using Application.Features.ChatGroups.Queries.GetAll;
using Application.Features.Messages.Commands.Create;
using Application.Features.Messages.Commands.Update;
using Application.Features.Messages.Queries.GetChatGroupMessages;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("_api/[controller]")]
[ApiController]
public sealed class MessagesController : BaseController
{
    
    [HttpGet]
    public async Task<IActionResult> GetAll(GetChatGroupMessagesQueryRequest getChatGroupMessagesQueryRequest)
    {
        var response = await Mediator.Send(getChatGroupMessagesQueryRequest);
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateMessageCommandRequest createMessageCommandRequest)
    {
        var response = await Mediator.Send(createMessageCommandRequest);
        return Ok(response);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UpdateMessageCommandRequest updateMessageCommandRequest)
    {
        var response = await Mediator.Send(updateMessageCommandRequest);
        return Ok(response);
    }
}