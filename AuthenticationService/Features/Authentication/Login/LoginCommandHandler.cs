using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Authentication.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginManager _loginManager;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpContextAccessor = httpContextAccessor;
            _loginManager = new LoginManager(unitOfWork, passwordHasher);
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (request.LoginRequest is null)
                throw new ArgumentNullException(nameof(request.LoginRequest), "Login request cannot be null.");

            var email = request.LoginRequest.Email.Trim().ToLower();
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            // 1. Validate User and Lockout policy
            User user;
            try
            {
                user = await _loginManager.ValidateUserAndCheckLockoutAsync(email, cancellationToken);
            }
            catch (AppException ex) when (ex.ErrorCode == "AUTH_INVALID_CREDENTIALS")
            {
                await _loginManager.LogAnonymousFailedAttemptAsync(email, ipAddress, cancellationToken);
                throw;
            }

            // 2. Verify Credentials
            var (verified, rehashNeeded, newHash) = _passwordHasher.VerifyPassword(request.LoginRequest.Password, user.PasswordHash);

            if (!verified)
            {
                await _loginManager.HandleFailedLoginAsync(user, email, ipAddress, cancellationToken);
            }

            // 3. Process Successful Authentication
            await _loginManager.HandleSuccessfulLoginAsync(user, email, ipAddress, rehashNeeded ? newHash : null, cancellationToken);

            // 4. Generate Access & Refresh Tokens
            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshTokenString = GenerateSecureToken();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsDeleted = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshTokenString,
                ProfileCompleted = user.ProfileCompleted,
                IsPremium = false
            };
        }

        private static string GenerateSecureToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
