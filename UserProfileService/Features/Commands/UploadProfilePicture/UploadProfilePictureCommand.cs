using FitnessApp.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitnessApp.UserProfileService.Features.Commands.UploadProfilePicture
{
    public record UploadProfilePictureCommand(int UserId, IFormFile ProfilePicture) : IRequest<ApiResponse<string>>;
}
