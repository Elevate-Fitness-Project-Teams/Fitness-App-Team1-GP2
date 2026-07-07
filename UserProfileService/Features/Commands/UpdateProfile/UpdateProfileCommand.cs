using FitnessApp.Shared.Models;
using MediatR;

namespace FitnessApp.UserProfileService.Features.Commands.UpdateProfile
{
    public record UpdateProfileCommand(
        int UserId,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber) : IRequest<ApiResponse<string>>;
}
