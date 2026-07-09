using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Domain.Contracts
{
    public interface IUserProfileRepository
    {
        IQueryable<UserProfile> GetQueryable(bool asNoTracking = false);
        
        Task<TResult?> GetProjectedProfileAsync<TResult>(
            int userId, 
            Expression<Func<UserProfile, TResult>> selector, 
            CancellationToken cancellationToken = default);

        Task<UserProfile?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserProfile> AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default);
        Task DeleteAsync(UserProfile profile, CancellationToken cancellationToken = default);
    }
}
