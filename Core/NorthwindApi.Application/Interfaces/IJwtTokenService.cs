
namespace NorthwindApi.Application.Interfaces;
public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string email);
    string GenerateRefreshToken();
    bool VerifyPassword(string password, string passwordHash);

}