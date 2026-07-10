using FitnessApp.Shared.Exceptions;
using FitnessApp.Shared.Models;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Shared.Events;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPublishEndpoint _publishEndpoint;

        public RegisterCommandHandler(
            IUnitOfWork unitOfWork, 
            IPasswordHasher passwordHasher,
            IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<RegisterDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterRequest is null)
                throw new ArgumentNullException(nameof(request.RegisterRequest), "Register request cannot be null.");

            var email = request.RegisterRequest.Email.Trim().ToLower();

            var existingUser = await _unitOfWork.Users.GetQueryable(ignoreQueryFilters: true)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (existingUser != null)
            {
                throw new DuplicateEmailException(email);
            }

            var hashedPassword = _passwordHasher.HashPassword(request.RegisterRequest.Password);

            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                isLockedOut = false,
                CreatedAt = DateTime.UtcNow,
                ProfileCompleted = true
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userRegisteredEvent = new UserRegisteredEvent(
                user.Id,
                request.RegisterRequest.FirstName,
                request.RegisterRequest.LastName,
                email,
                request.RegisterRequest.PhoneNumber
            );

            await _publishEndpoint.Publish(userRegisteredEvent, cancellationToken);

            return new RegisterDto
            {
                UserId = user.Id
            };
        }
    }
}
