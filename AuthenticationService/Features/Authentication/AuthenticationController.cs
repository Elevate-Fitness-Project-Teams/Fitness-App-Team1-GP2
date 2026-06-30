using AuthenticationService.Features.Authentication.Register;
using AuthenticationService.Common.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    }
}