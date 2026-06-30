using AuthenticationService.Common.Exceptions;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Authentication.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterRequest is null)
                throw new ArgumentNullException(nameof(request.RegisterRequest), "Register request cannot be null.");

            var email = request.RegisterRequest.Email.Trim().ToLower();

            // Check if the email exists in the database
            var existingUser = await _userRepository.GetByEmailAsync(email, ignoreQueryFilters: true, cancellationToken);

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
                CreatedAt = DateTime.UtcNow
            };

            // Save to database via UserRepository
            await _userRepository.AddAsync(user, cancellationToken);

            // Return response dto
            return new RegisterDto
            {
                UserId = user.Id,
                RequiresProfileCompletion = true
            };
        }
    }
}
