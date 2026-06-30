using MediatR;

namespace AuthenticationService.Features.Authentication.Register
{
    public record RegisterCommand(RegisterRequest RegisterRequest) : IRequest<RegisterDto>;
}
