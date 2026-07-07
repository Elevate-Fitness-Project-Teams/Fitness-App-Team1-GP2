using FitnessApp.Shared.Models;
using FitnessApp.UserProfileService.Features.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.UserProfileService.Features
{
    [ApiController]
    [Route("api/v1")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                var errorResponse = ApiResponse<GetProfileDto>.Failure(
                    new List<string> { "AUTH_TOKEN_INVALID" },
                    "Invalid or missing user authentication token.",
                    401);
                return StatusCode(401, errorResponse);
            }

            var query = new GetProfileQuery(userId);
            var result = await _mediator.Send(query);

            return StatusCode(result.StatusCode, result);
        }
    }
}
