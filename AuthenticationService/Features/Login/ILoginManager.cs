using AuthenticationService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Features.Login
{
    public interface ILoginManager
    {
        Task<User> ValidateUserAndCheckLockoutAsync(string email, CancellationToken cancellationToken);
        Task HandleFailedLoginAsync(User user, string email, string ipAddress, CancellationToken cancellationToken);
        Task HandleSuccessfulLoginAsync(User user, string email, string ipAddress, string? newHash, CancellationToken cancellationToken);
        Task LogAnonymousFailedAttemptAsync(string email, string ipAddress, CancellationToken cancellationToken);
    }
}
