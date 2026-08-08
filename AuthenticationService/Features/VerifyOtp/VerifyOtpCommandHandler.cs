using FitnessApp.Shared.Exceptions;
using AuthenticationService.Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMemoryCache _memoryCache;

        public VerifyOtpCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _memoryCache = memoryCache;
        }

        public async Task<VerifyOtpDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            if (request.VerifyOtpRequest is null)
                throw new ArgumentNullException(nameof(request.VerifyOtpRequest), "Request payload cannot be null.");

            var email = request.VerifyOtpRequest.Email.Trim().ToLowerInvariant();
            var otp = request.VerifyOtpRequest.Otp;

            var otpRecord = await _unitOfWork.OtpCodes.GetQueryable(ignoreQueryFilters: false)
                .Where(o => o.Email == email && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (otpRecord is null)
            {
                throw new AppException("Invalid OTP.", System.Net.HttpStatusCode.BadRequest, "AUTH_INVALID_OTP");
            }

            if (otpRecord.FailedAttempts >= 3)
            {
                if (!otpRecord.IsUsed)
                {
                    otpRecord.IsUsed = true;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                throw new AppException("Too many failed attempts. Please request a new OTP.", System.Net.HttpStatusCode.BadRequest, "AUTH_OTP_LOCKED");
            }

            var verificationResult = _passwordHasher.VerifyPassword(otp, otpRecord.Code);
            
            if (!verificationResult.Verified)
            {
                otpRecord.FailedAttempts++;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new AppException("Invalid OTP.", System.Net.HttpStatusCode.BadRequest, "AUTH_INVALID_OTP");
            }

            if (otpRecord.ExpiresAt < DateTime.UtcNow)
            {
                throw new AppException("OTP has expired.", System.Net.HttpStatusCode.BadRequest, "AUTH_OTP_EXPIRED");
            }

            otpRecord.IsUsed = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resetToken = Guid.NewGuid().ToString("N");

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            _memoryCache.Set($"reset-{resetToken}", email, cacheOptions);

            return new VerifyOtpDto
            {
                ResetToken = resetToken
            };
        }
    }
}
