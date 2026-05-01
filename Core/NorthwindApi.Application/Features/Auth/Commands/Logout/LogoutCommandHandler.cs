using MediatR;
using NorthwindApi.Application.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly ITokenBlacklistService _blacklistService;
        private readonly IJwtTokenService _jwtTokenService;

        public LogoutCommandHandler(
            ITokenBlacklistService blacklistService,
            IJwtTokenService jwtTokenService)
        {
            _blacklistService = blacklistService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Access token blacklist
            var accessJti = _jwtTokenService.GetJtiFromToken(request.AccessToken);
            var accessExpiry = _jwtTokenService.GetExpiryFromToken(request.AccessToken);

            if (accessJti != null && accessExpiry.HasValue)
                await _blacklistService.BlacklistTokenAsync(accessJti, accessExpiry.Value, cancellationToken);

            // Refresh token blacklist (hash alarak sakla)
            var refreshTokenHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(request.RefreshToken)));

            await _blacklistService.BlacklistTokenAsync(
                refreshTokenHash,
                DateTime.UtcNow.AddDays(7), // Refresh token süresi
                cancellationToken);
        }
    }
}
