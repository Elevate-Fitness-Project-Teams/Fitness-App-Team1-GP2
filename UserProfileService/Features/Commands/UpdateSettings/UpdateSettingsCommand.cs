using FitnessApp.Shared.Models;
using MediatR;

namespace FitnessApp.UserProfileService.Features.Commands.UpdateSettings
{
    public record UpdateSettingsCommand(int UserId, UpdateSettingsRequest Request) : IRequest<ApiResponse<bool>>;
}
