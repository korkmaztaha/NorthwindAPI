using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NorthwindApi.Application.Features.Auth.Commands.Login;
using NorthwindApi.Application.Features.Auth.Commands.Logout;

namespace NorthwindAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthPolicy")]
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
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    //[HttpGet("hash-password")]
    //public IActionResult HashPassword([FromQuery] string password)
    //{
    //    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    //    return Ok(hash);
    //}
}