
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Repositories;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _logger = logger;

    }

    public async Task<LoginCommandResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = _unitOfWork.Repository<User>()
            .GetAll()
            .FirstOrDefault(x => x.Email == request.Email);

        if (user is null || !_jwtTokenService.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Giriş başarısız: {Email}", request.Email);
            throw new UnauthorizedAccessException("Geçersiz kullanıcı adı veya şifre");
        }

        _logger.LogInformation("Başarılı login: {Email}, Role: {Role}", user.Email, user.Role);
        // Access token üret
        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id.ToString(), user.Email,user.Role);

        // Refresh token üret
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Refresh token'ı DB'ye kaydet
        await _unitOfWork.Repository<RefreshTokens>().AddAsync(new RefreshTokens
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginCommandResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }
}