using MediatR;

namespace AuthenticationService.Features.VerifyOtp
{
    public record VerifyOtpCommand(VerifyOtpRequest VerifyOtpRequest) : IRequest<VerifyOtpDto>;
}
