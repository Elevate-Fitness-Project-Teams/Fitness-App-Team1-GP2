using MediatR;

namespace AuthenticationService.Features.Authentication.Logout
{
    public record LogoutCommand(int UserId) : IRequest<LogoutViewModel>;
}
