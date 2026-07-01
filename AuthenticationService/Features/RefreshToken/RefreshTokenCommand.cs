using MediatR;

namespace AuthenticationService.Features.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequest RefreshTokenRequest) : IRequest<RefreshTokenResponse>;
}
