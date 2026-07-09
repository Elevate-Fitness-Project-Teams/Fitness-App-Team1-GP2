using AuthenticationService.Domain.Contracts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenDto>
    {
        private readonly RefreshTokenManager _refreshTokenManager;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _refreshTokenManager = new RefreshTokenManager(unitOfWork, tokenService);
        }

        public async Task<RefreshTokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (request.RefreshTokenRequest is null)
                throw new ArgumentNullException(nameof(request.RefreshTokenRequest), "Refresh token request cannot be null.");

            return await _refreshTokenManager.RefreshAsync(request.RefreshTokenRequest.RefreshToken, cancellationToken);
        }
    }
}
