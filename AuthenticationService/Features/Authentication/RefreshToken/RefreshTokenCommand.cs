using MediatR;

namespace AuthenticationService.Features.Authentication.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequest RefreshTokenRequest) : IRequest<RefreshTokenResponse>;
}
