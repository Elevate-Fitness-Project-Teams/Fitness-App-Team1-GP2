using MediatR;

namespace AuthenticationService.Features.Register
{
    public record RegisterCommand(RegisterRequest RegisterRequest) : IRequest<RegisterDto>;
}
