using NorthwindApi.Application.Interfaces.Infrastructure;
using System.IdentityModel.Tokens.Jwt;

namespace NorthwindAPI.Api.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                    if (jti != null && await blacklistService.IsTokenBlacklistedAsync(jti))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = 401,
                            Message = "Token has been revoked.",
                            Timestamp = DateTime.UtcNow
                        });
                        return;
                    }
                }
                catch
                {
                    // Token parse edilemezse devam et, JWT middleware halleder
                }
            }

            await _next(context);
        }
    }
}