using System;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;
using UserProfileService.Infrastructure.Persistence.Context;

namespace UserProfileService.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserProfileDbContext _dbContext;
        private IUserProfileRepository? _userProfiles;
        private bool _disposed;

        public UnitOfWork(UserProfileDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IUserProfileRepository UserProfiles => 
            _userProfiles ??= new UserProfileRepository(_dbContext);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
