using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NorthwindApi.Application.Features.Auth.Commands.Login;
using NorthwindApi.Application.Features.Auth.Commands.Logout;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Kimlik doğrulama işlemleri
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Kullanıcı girişi yapar ve token döner
    /// </summary>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Kullanıcı girişi",
        Description = "Email ve şifre ile giriş yaparak Access Token ve Refresh Token alır",
        OperationId = "Login",
        Tags = new[] { "Auth" })]
    [SwaggerResponse(200, "Giriş başarılı, token döndürüldü")]
    [SwaggerResponse(400, "Geçersiz email veya şifre")]
    [SwaggerResponse(429, "Çok fazla istek - brute force koruması")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcı çıkışı yapar ve token'ı geçersiz kılar
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [SwaggerOperation(
        Summary = "Kullanıcı çıkışı",
        Description = "Access Token ve Refresh Token'ı blacklist'e ekler",
        OperationId = "Logout",
        Tags = new[] { "Auth" })]
    [SwaggerResponse(204, "Çıkış başarılı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}