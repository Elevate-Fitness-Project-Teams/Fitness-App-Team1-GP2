using MediatR;

namespace AuthenticationService.Features.ChangePassword
{
    public record ChangePasswordCommand(int UserId, ChangePasswordRequest Request) : IRequest<ChangePasswordDto>;
}
