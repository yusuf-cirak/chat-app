using Application.Features.Auths.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : BaseController
{
    [HttpPost("register")]
      public async Task<IActionResult> Register(RegisterCommandRequest registerCommandRequest)
      {
          var response = await Mediator.Send(registerCommandRequest);
          return Ok(response);
      }
}