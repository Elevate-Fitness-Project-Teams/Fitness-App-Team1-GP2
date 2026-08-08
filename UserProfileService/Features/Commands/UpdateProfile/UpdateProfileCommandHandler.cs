using FitnessApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;
using MassTransit;
using FitnessApp.Shared.Events;
namespace FitnessApp.UserProfileService.Features.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ApiResponse<string>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
            {
                return ApiResponse<string>.Failure(
                    new List<string> { "AUTH_TOKEN_INVALID" },
                    "Invalid or missing user authentication token.",
                    401);
            }

            var profile = await _unitOfWork.UserProfiles.GetByIdAsync(request.UserId, cancellationToken);

            if (profile == null)
            {
                return ApiResponse<string>.Failure(
                    new List<string> { "RES_NOT_FOUND" },
                    "User profile was not found.",
                    404);
            }

            profile.FirstName = request.FirstName;
            profile.LastName = request.LastName;
            profile.PhoneNumber = request.PhoneNumber;

            await _unitOfWork.UserProfiles.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userProfileUpdatedEvent = new UserProfileUpdatedEvent(
                profile.UserId,
                profile.FirstName,
                profile.LastName,
                profile.PhoneNumber,
                profile.ProfilePictureUrl
            );
            await _publishEndpoint.Publish(userProfileUpdatedEvent, cancellationToken);

            return ApiResponse<string>.Success("Profile updated successfully.", "Success");
        }
    }
}
