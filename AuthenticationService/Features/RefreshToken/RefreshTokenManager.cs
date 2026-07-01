using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.RefreshToken
{
    public class RefreshTokenManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public RefreshTokenManager(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<RefreshTokenDto> RefreshAsync(string tokenString, CancellationToken cancellationToken)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetQueryable(ignoreQueryFilters: false)
                .FirstOrDefaultAsync(t => t.Token == tokenString, cancellationToken);

            if (storedToken == null)
            {
                throw new AppException("Invalid refresh token.", HttpStatusCode.Unauthorized, "AUTH_INVALID_TOKEN");
            }

            if (storedToken.RevokedAt.HasValue)
            {
                throw new AppException("Refresh token has been revoked.", HttpStatusCode.Unauthorized, "AUTH_TOKEN_REVOKED");
            }

            if (storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new AppException("Refresh token has expired.", HttpStatusCode.Unauthorized, "AUTH_TOKEN_EXPIRED");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId, ignoreQueryFilters: false, cancellationToken);

            if (user == null)
            {
                throw new AppException("User associated with token not found.", HttpStatusCode.Unauthorized, "AUTH_USER_NOT_FOUND");
            }

            // Revoke current token
            storedToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshTokens.UpdateAsync(storedToken, cancellationToken);

            // Generate new token pair
            var (accessToken, newRefreshTokenEntity) = _tokenService.GenerateTokens(user);

            // Save new refresh token
            await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenDto
            {
                Token = accessToken,
                RefreshToken = newRefreshTokenEntity.Token
            };
        }
    }
}
