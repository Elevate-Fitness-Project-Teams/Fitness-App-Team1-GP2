using FitnessApp.Shared.Models;
using FitnessApp.UserProfileService.Domain.Contracts;
using UserProfileService.Domain.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using FitnessApp.Shared.Events;

namespace FitnessApp.UserProfileService.Features.Commands.UploadProfilePicture
{
    public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPublishEndpoint _publishEndpoint;

        public UploadProfilePictureCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ApiResponse<string>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
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

            string fileUrl;
            try
            {
                fileUrl = await _fileStorageService.SaveFileAsync(request.ProfilePicture, "profile-pictures", cancellationToken);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure(
                    new List<string> { "SRV_FILE_UPLOAD_FAILED" },
                    $"File upload failed due to a technical error: {ex.Message}",
                    500);
            }

            profile.ProfilePictureUrl = fileUrl;

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

            return ApiResponse<string>.Success(fileUrl, "Profile picture updated successfully.");
        }
    }
}
