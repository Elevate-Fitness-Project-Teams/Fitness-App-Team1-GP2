using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMemoryCache _memoryCache;

        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _memoryCache = memoryCache;
        }

        public async Task<ResetPasswordDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.ResetPasswordRequest is null)
                throw new ArgumentNullException(nameof(request.ResetPasswordRequest), "Request payload cannot be null.");

            var resetToken = request.ResetPasswordRequest.ResetToken;
            var newPassword = request.ResetPasswordRequest.NewPassword;

            //Verify token in cache
            var cacheKey = $"reset-{resetToken}";
            if (!_memoryCache.TryGetValue(cacheKey, out string? email) || string.IsNullOrEmpty(email))
            {
                throw new AppException("Invalid or expired reset token.", System.Net.HttpStatusCode.BadRequest, "AUTH_RESET_TOKEN_INVALID");
            }

            var user = await _unitOfWork.Users.GetQueryable(ignoreQueryFilters: false)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
            {
                throw new AppException("User not found.", System.Net.HttpStatusCode.NotFound, "RES_NOT_FOUND");
            }

            var newHash = _passwordHasher.HashPassword(newPassword);
            user.PasswordHash = newHash;

            // 4. Revoke active refresh tokens to force global logout
            var activeRefreshTokens = await _unitOfWork.RefreshTokens.GetQueryable(ignoreQueryFilters: false)
                .Where(t => t.UserId == user.Id && !t.RevokedAt.HasValue && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var token in activeRefreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            // remove token from cache
            _memoryCache.Remove(cacheKey);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResetPasswordDto
            {
                PasswordChanged = true
            };
        }
    }
}
