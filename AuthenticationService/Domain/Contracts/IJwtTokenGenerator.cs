using AuthenticationService.Domain.Entities;

namespace AuthenticationService.Domain.Contracts
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
