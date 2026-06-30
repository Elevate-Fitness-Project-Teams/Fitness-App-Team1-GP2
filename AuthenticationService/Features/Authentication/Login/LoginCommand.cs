using MediatR;

namespace AuthenticationService.Features.Authentication.Login
{
    public record LoginCommand(LoginRequest LoginRequest) : IRequest<LoginResponse>;
}
