using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;
using UserProfileService.Domain.Entities;
using UserProfileService.Infrastructure.Persistence.Context;

namespace UserProfileService.Infrastructure.Persistence.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly UserProfileDbContext _dbContext;
        private readonly DbSet<UserProfile> _dbSet;

        public UserProfileRepository(UserProfileDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<UserProfile>();
        }

        public IQueryable<UserProfile> GetQueryable(bool asNoTracking = false)
        {
            var query = _dbSet.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public async Task<TResult?> GetProjectedProfileAsync<TResult>(
            int userId, 
            Expression<Func<UserProfile, TResult>> selector, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<UserProfile?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        public async Task<UserProfile> AddAsync(UserProfile profile, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(profile, cancellationToken);
            return profile;
        }

        public async Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(profile).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(UserProfile profile, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(profile);
            await Task.CompletedTask;
        }
    }
}
