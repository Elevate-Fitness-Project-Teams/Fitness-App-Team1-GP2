using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Authentication.Login
{
    public class LoginManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public LoginManager(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> ValidateUserAndCheckLockoutAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetQueryable(ignoreQueryFilters: false)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user == null)
            {
                throw new AppException("Invalid email or password.", HttpStatusCode.Unauthorized, "AUTH_INVALID_CREDENTIALS");
            }

            if (user.isLockedOut && user.LockedUntil.HasValue)
            {
                if (user.LockedUntil.Value > DateTime.UtcNow)
                {
                    throw new AppException(
                        $"Account is locked out. Please try again after {user.LockedUntil.Value:yyyy-MM-dd HH:mm:ss} UTC.", 
                        HttpStatusCode.Locked, 
                        "AUTH_ACCOUNT_LOCKED"
                    );
                }
                else
                {
                    // Lock expired - reset lockout properties
                    user.isLockedOut = false;
                    user.LockedUntil = null;
                    await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            return user;
        }

        public async Task HandleFailedLoginAsync(User user, string email, string ipAddress, CancellationToken cancellationToken)
        {
            var failedAttempt = new LoginAttempt
            {
                Email = email,
                AttemptedAt = DateTime.UtcNow,
                IsSuccess = false,
                IpAddress = ipAddress
            };
            await _unitOfWork.LoginAttempts.AddAsync(failedAttempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var cutoff = DateTime.UtcNow.AddMinutes(-15);
            var failedCount = await _unitOfWork.LoginAttempts.GetQueryable()
                .CountAsync(x => x.Email == email && !x.IsSuccess && x.AttemptedAt >= cutoff, cancellationToken);

            if (failedCount >= 5)
            {
                user.isLockedOut = true;
                user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new AppException(
                    "Account locked. Too many failed attempts. Please try again in 15 minutes.", 
                    HttpStatusCode.Locked, 
                    "AUTH_ACCOUNT_LOCKED"
                );
            }

            throw new AppException("Invalid email or password.", HttpStatusCode.Unauthorized, "AUTH_INVALID_CREDENTIALS");
        }

        public async Task HandleSuccessfulLoginAsync(User user, string email, string ipAddress, string? newHash, CancellationToken cancellationToken)
        {
            // Reset lockout metrics
            if (user.isLockedOut || user.LockedUntil.HasValue)
            {
                user.isLockedOut = false;
                user.LockedUntil = null;
            }

            // Update PasswordHash if rehash is needed
            if (!string.IsNullOrEmpty(newHash))
            {
                user.PasswordHash = newHash;
            }

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);

            // Log successful login attempt
            var successAttempt = new LoginAttempt
            {
                Email = email,
                AttemptedAt = DateTime.UtcNow,
                IsSuccess = true,
                IpAddress = ipAddress
            };
            await _unitOfWork.LoginAttempts.AddAsync(successAttempt, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task LogAnonymousFailedAttemptAsync(string email, string ipAddress, CancellationToken cancellationToken)
        {
            var dummyAttempt = new LoginAttempt
            {
                Email = email,
                AttemptedAt = DateTime.UtcNow,
                IsSuccess = false,
                IpAddress = ipAddress
            };
            await _unitOfWork.LoginAttempts.AddAsync(dummyAttempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
