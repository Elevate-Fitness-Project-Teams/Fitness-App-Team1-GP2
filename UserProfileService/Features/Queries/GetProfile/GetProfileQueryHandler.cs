using FitnessApp.Shared.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;

namespace FitnessApp.UserProfileService.Features.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ApiResponse<GetProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<GetProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
            {
                return ApiResponse<GetProfileDto>.Failure(
                    new List<string> { "AUTH_TOKEN_INVALID" },
                    "User authentication token is invalid.",
                    401);
            }

            var profileDto = await _unitOfWork.UserProfiles.GetProjectedProfileAsync(
                request.UserId,
                p => new GetProfileDto
                {
                    UserId = p.UserId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    ProfilePictureUrl = p.ProfilePictureUrl,
                    IsPremiumCached = p.IsPremiumCached,
                    MemberSince = p.MemberSince,
                    TotalWorkouts = p.Statistics != null ? p.Statistics.TotalWorkouts : 0,
                    CurrentStreak = p.Statistics != null ? p.Statistics.CurrentStreak : 0
                },
                cancellationToken);

            if (profileDto == null)
            {
                return ApiResponse<GetProfileDto>.Failure(
                    new List<string> { "RES_NOT_FOUND" },
                    "User profile was not found.",
                    404);
            }

            return ApiResponse<GetProfileDto>.Success(profileDto);
        }
    }
}
