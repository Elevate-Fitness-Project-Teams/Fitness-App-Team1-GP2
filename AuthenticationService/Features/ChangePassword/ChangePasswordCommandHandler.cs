using AuthenticationService.Domain.Contracts;
using FitnessApp.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<ChangePasswordDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.Request is null)
                throw new ArgumentNullException(nameof(request.Request), "Request payload cannot be null.");

            var currentPassword = request.Request.CurrentPassword;
            var newPassword = request.Request.NewPassword;

            var user = await _unitOfWork.Users.GetQueryable(ignoreQueryFilters: false)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new AppException("User not found.", System.Net.HttpStatusCode.NotFound, "RES_NOT_FOUND");
            }

            var verificationResult = _passwordHasher.VerifyPassword(currentPassword, user.PasswordHash);
            if (!verificationResult.Verified)
            {
                throw new AppException("Incorrect current password.", System.Net.HttpStatusCode.Unauthorized, "AUTH_INVALID_CREDENTIALS");
            }

            var newHash = _passwordHasher.HashPassword(newPassword);
            user.PasswordHash = newHash;

            var activeRefreshTokens = await _unitOfWork.RefreshTokens.GetQueryable(ignoreQueryFilters: false)
                .Where(t => t.UserId == user.Id && !t.RevokedAt.HasValue && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var token in activeRefreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ChangePasswordDto
            {
                PasswordChanged = true
            };
        }
    }
}
