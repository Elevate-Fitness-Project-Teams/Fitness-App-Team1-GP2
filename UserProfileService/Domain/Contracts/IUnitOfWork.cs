using System;
using System.Threading;
using System.Threading.Tasks;

namespace UserProfileService.Domain.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
