using MediatR;

namespace AuthenticationService.Features.Logout
{
    public record LogoutCommand(int UserId) : IRequest<LogoutViewModel>;
}
