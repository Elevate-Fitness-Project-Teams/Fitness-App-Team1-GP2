using AuthenticationService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<OtpCode> OtpCodes { get; }
        IGenericRepository<RefreshToken> RefreshTokens { get; }
        IGenericRepository<LoginAttempt> LoginAttempts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
