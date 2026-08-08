using FitnessApp.Shared.Models;
using MediatR;

namespace FitnessApp.UserProfileService.Features.Queries.GetSettings
{
    public record GetSettingsQuery(int UserId) : IRequest<ApiResponse<GetSettingsDto>>;
}
