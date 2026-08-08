using MediatR;

namespace AuthenticationService.Features.ForgotPassword
{
    public record ForgotPasswordCommand(ForgotPasswordRequest ForgotPasswordRequest) : IRequest<ForgotPasswordDto>;
}
