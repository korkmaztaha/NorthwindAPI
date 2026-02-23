using MediatR;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Auth.Commands.RefreshToken
{
    
    public class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<RefreshTokenCommandResponse> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            // DB'den refresh token'ı bul
            var refreshToken = _unitOfWork.Repository<RefreshTokens>()
                .GetAll()
                .FirstOrDefault(x => x.Token == request.RefreshToken);

            // Geçerlilik kontrolleri
            if (refreshToken is null)
                throw new UnauthorizedAccessException("Geçersiz refresh token");

            if (refreshToken.IsRevoked)
                throw new UnauthorizedAccessException("Refresh token iptal edilmiş");

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token süresi dolmuş");

            // Kullanıcıyı bul
            var user = _unitOfWork.Repository<User>()
                .GetAll()
                .FirstOrDefault(x => x.Id == refreshToken.UserId);

            if (user is null)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı");

            // Eski refresh token'ı iptal et
            refreshToken.IsRevoked = true;
            _unitOfWork.Repository<RefreshTokens>().Update(refreshToken);

            // Yeni tokenları üret
            var newAccessToken = _jwtTokenService.GenerateAccessToken(
                user.Id.ToString(), user.Email,user.Role);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // Yeni refresh token'ı DB'ye kaydet
            await _unitOfWork.Repository<RefreshTokens>().AddAsync(new RefreshTokens
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenCommandResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
