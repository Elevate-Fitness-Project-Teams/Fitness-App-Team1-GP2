using FitnessApp.Shared.Exceptions;
using FitnessApp.Shared.Models;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.CompleteProfile
{
    public class CompleteProfileCommandHandler : IRequestHandler<CompleteProfileCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly IPublishEndpoint _publishEndpoint;

        public CompleteProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ignoreQueryFilters: false, cancellationToken);

            if (user == null)
            {
                throw new AppException("User not found.", HttpStatusCode.NotFound, "AUTH_USER_NOT_FOUND");
            }

            if (user.ProfileCompleted)
            {
                throw new AppException("Profile is already completed.", HttpStatusCode.BadRequest, "AUTH_PROFILE_ALREADY_COMPLETED");
            }

            var cacheKey = $"reg-{request.UserId}";
            if (!_cache.TryGetValue(cacheKey, out CachedRegistrationData? cachedData) || cachedData == null)
            {
                throw new AppException(
                    "Profile completion session expired or not found. Please register again.", 
                    HttpStatusCode.BadRequest, 
                    "AUTH_REGISTRATION_EXPIRED"
                );
            }

            user.ProfileCompleted = true;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var integrationEvent = new ProfileLifecycleInitiatedEvent(
                user.Id,
                cachedData.FirstName,
                cachedData.LastName,
                cachedData.Email,
                cachedData.PhoneNumber
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);

            _cache.Remove(cacheKey);
        }
    }
}
