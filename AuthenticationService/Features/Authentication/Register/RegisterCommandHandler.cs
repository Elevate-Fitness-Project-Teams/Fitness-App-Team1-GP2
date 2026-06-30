using AuthenticationService.Common.Exceptions;
using AuthenticationService.Common.Shared;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Authentication.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMemoryCache _cache;

        public RegisterCommandHandler(
            IUnitOfWork unitOfWork, 
            IPasswordHasher passwordHasher,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _cache = cache;
        }

        public async Task<RegisterDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterRequest is null)
                throw new ArgumentNullException(nameof(request.RegisterRequest), "Register request cannot be null.");

            var email = request.RegisterRequest.Email.Trim().ToLower();

            // Check if the email exists in the database (even if soft-deleted)
            var existingUser = await _unitOfWork.Users.GetQueryable(ignoreQueryFilters: true)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (existingUser != null)
            {
                throw new DuplicateEmailException(email);
            }

            // Hash the password
            var hashedPassword = _passwordHasher.HashPassword(request.RegisterRequest.Password);

            // Create new User entity
            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                isLockedOut = false,
                CreatedAt = DateTime.UtcNow,
                ProfileCompleted = false
            };

            // Save to database via UnitOfWork
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Cache registration profile details for life-cycle completion (30-minute sliding expiration)
            var cacheKey = $"reg-{user.Id}";
            var cachedData = new CachedRegistrationData(
                request.RegisterRequest.FirstName,
                request.RegisterRequest.LastName,
                email,
                request.RegisterRequest.PhoneNumber
            );
            
            _cache.Set(cacheKey, cachedData, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            // Return response dto
            return new RegisterDto
            {
                UserId = user.Id,
                RequiresProfileCompletion = true
            };
        }
    }
}
