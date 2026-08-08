using AuthenticationService.Domain.Entities;

namespace AuthenticationService.Domain.Contracts
{
    public interface ITokenService
    {
        (string AccessToken, RefreshToken RefreshToken) GenerateTokens(User user);
    }
}
