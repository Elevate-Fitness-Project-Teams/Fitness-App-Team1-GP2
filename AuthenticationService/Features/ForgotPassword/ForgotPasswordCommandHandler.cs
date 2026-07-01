using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public ForgotPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<ForgotPasswordDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.ForgotPasswordRequest is null)
                throw new ArgumentNullException(nameof(request.ForgotPasswordRequest), "Request payload cannot be null.");

            var email = request.ForgotPasswordRequest.Email.Trim().ToLowerInvariant();

            var userExists = await _unitOfWork.Users.GetQueryable(ignoreQueryFilters: false)
                .AnyAsync(u => u.Email == email, cancellationToken);

            // If user doesn't exist, skip and return success to prevent enumeration
            if (!userExists)
            {
                return new ForgotPasswordDto
                {
                    Email = email,
                    OtpExpiresIn = 600,
                    CanResendIn = 30
                };
            }

            // (30 seconds)
            var thirtySecondsAgo = DateTime.UtcNow.AddSeconds(-30);
            
            var recentOtpExists = await _unitOfWork.OtpCodes.GetQueryable(ignoreQueryFilters: false)
                .AnyAsync(o => o.Email == email && o.CreatedAt >= thirtySecondsAgo, cancellationToken);

            if (recentOtpExists)
            {
                throw new AppException("Please wait before requesting another OTP.", System.Net.HttpStatusCode.TooManyRequests, "RATE_OTP_RESEND_TOO_SOON");
            }

            //Code Generation
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString();

            var hashedCode = _passwordHasher.HashPassword(otpCode);

            var otpEntity = new OtpCode
            {
                Email = email,
                Code = hashedCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.OtpCodes.AddAsync(otpEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //send the OTP via Email

            return new ForgotPasswordDto
            {
                Email = email,
                OtpExpiresIn = 600,
                CanResendIn = 30
            };
        }
    }
}
