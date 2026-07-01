using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginManager _loginManager;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _loginManager = new LoginManager(unitOfWork, passwordHasher);
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (request.LoginRequest is null)
                throw new ArgumentNullException(nameof(request.LoginRequest), "Login request cannot be null.");

            var email = request.LoginRequest.Email.Trim().ToLower();
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

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

            var (verified, rehashNeeded, newHash) = _passwordHasher.VerifyPassword(request.LoginRequest.Password, user.PasswordHash);

            if (!verified)
            {
                await _loginManager.HandleFailedLoginAsync(user, email, ipAddress, cancellationToken);
            }

            await _loginManager.HandleSuccessfulLoginAsync(user, email, ipAddress, rehashNeeded ? newHash : null, cancellationToken);

            var (accessToken, refreshTokenEntity) = _tokenService.GenerateTokens(user);

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                ProfileCompleted = user.ProfileCompleted,
                IsPremium = false
            };
        }
    }
}
