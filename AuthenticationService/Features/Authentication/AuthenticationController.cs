using AuthenticationService.Features.Authentication.Register;
using AuthenticationService.Features.Authentication.Login;
using AuthenticationService.Features.Authentication.CompleteProfile;
using AuthenticationService.Common.Shared;
using AuthenticationService.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationService.Features.Authentication
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request);
            var result = await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<RegisterDto>.Success(result, "Registration successful. Please complete your profile.", 201);

            return Created(string.Empty, response);
        }

        [HttpPost("complete-profile")]
        [Authorize]
        public async Task<IActionResult> CompleteProfile(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new AppException("Unauthorized access.", System.Net.HttpStatusCode.Unauthorized, "AUTH_UNAUTHORIZED");
            }

            var command = new CompleteProfileCommand(userId);
            await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<object?>.Success(null, "Profile lifecycle initiated.", 200);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request);
            var result = await _mediator.Send(command, cancellationToken);

            // Map DTO to ViewModel to comply with architectural boundaries
            var viewModel = new LoginViewModel
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken,
                ProfileCompleted = result.ProfileCompleted,
                IsPremium = result.IsPremium
            };

            var response = ApiResponse<LoginViewModel>.Success(viewModel, "Login successful.", 200);

            return Ok(response);
        }
    }
}