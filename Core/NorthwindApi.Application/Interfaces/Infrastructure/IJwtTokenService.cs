using System.Data;

namespace NorthwindApi.Application.Interfaces.Infrastructure;
public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string email,string role);
    string GenerateRefreshToken();
    bool VerifyPassword(string password, string passwordHash);
    string? GetJtiFromToken(string token);
    DateTime? GetExpiryFromToken(string token);


}