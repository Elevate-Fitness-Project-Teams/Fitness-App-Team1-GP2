using FitnessApp.Shared.Models;
using MediatR;

namespace FitnessApp.UserProfileService.Features.Queries.GetProfile
{
    public record GetProfileQuery(int UserId) : IRequest<ApiResponse<GetProfileDto>>;
}
