using MediatR;

namespace AuthenticationService.Features.Login
{
    public record LoginCommand(LoginRequest LoginRequest) : IRequest<LoginResponse>;
}
