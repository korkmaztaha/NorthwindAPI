using MediatR;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Auth.Commands.Login;

namespace NorthwindAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    //[HttpGet("hash-password")]
    //public IActionResult HashPassword([FromQuery] string password)
    //{
    //    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    //    return Ok(hash);
    //}
}