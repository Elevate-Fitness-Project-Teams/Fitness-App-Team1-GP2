using AuthenticationService.Common.Shared;
using AuthenticationService.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthenticationService.Features.Register;
using AuthenticationService.Features.Login;
using AuthenticationService.Features.Logout;
using AuthenticationService.Features.RefreshToken;
using AuthenticationService.Features.CompleteProfile;
using AuthenticationService.Features.ForgotPassword;

namespace AuthenticationService.Features
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

            var response = ApiResponse<LoginDto>.Success(result, "Login successful.", 200);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var command = new RefreshTokenCommand(request);
            var result = await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<RefreshTokenDto>.Success(result, "Token refreshed successfully.", 200);

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new AppException("Unauthorized access.", System.Net.HttpStatusCode.Unauthorized, "AUTH_UNAUTHORIZED");
            }

            var command = new LogoutCommand(userId);
            var result = await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<LogoutDto>.Success(result, "Logged out successfully.", 200);

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var command = new ForgotPasswordCommand(request);
            var result = await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<ForgotPasswordDto>.Success(result, "OTP generated successfully.", 200);

            return Ok(response);
        }
    }
}