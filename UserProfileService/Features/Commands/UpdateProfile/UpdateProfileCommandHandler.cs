using FitnessApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;

namespace FitnessApp.UserProfileService.Features.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            var emailExists = await _unitOfWork.UserProfiles
                .GetQueryable(asNoTracking: true)
                .AnyAsync(x => x.Email == request.Email && x.UserId != request.UserId, cancellationToken);

            if (emailExists)
            {
                return ApiResponse<string>.Failure(
                    new List<string> { "AUTH_EMAIL_EXISTS" },
                    "The provided email is already in use by another account.",
                    409);
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
            profile.Email = request.Email;
            profile.PhoneNumber = request.PhoneNumber;

            await _unitOfWork.UserProfiles.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.Success("Profile updated successfully.", "Success");
        }
    }
}
