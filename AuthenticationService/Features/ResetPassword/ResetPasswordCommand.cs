using MediatR;

namespace AuthenticationService.Features.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordRequest ResetPasswordRequest) : IRequest<ResetPasswordDto>;
}
