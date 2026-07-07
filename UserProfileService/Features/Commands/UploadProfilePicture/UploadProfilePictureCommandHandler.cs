using FitnessApp.Shared.Models;
using FitnessApp.UserProfileService.Domain.Contracts;
using UserProfileService.Domain.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.UserProfileService.Features.Commands.UploadProfilePicture
{
    public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public UploadProfilePictureCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
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

            return ApiResponse<string>.Success(fileUrl, "Profile picture updated successfully.");
        }
    }
}
