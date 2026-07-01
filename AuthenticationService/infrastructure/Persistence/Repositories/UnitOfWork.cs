using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using AuthenticationService.infrastructure.Persistence.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _dbContext;
        private IGenericRepository<User>? _users;
        private IGenericRepository<OtpCode>? _otpCodes;
        private IGenericRepository<RefreshToken>? _refreshTokens;
        private IGenericRepository<LoginAttempt>? _loginAttempts;
        private bool _disposed;

        public UnitOfWork(AuthDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IGenericRepository<User> Users => 
            _users ??= new GenericRepository<User>(_dbContext);

        public IGenericRepository<OtpCode> OtpCodes => 
            _otpCodes ??= new GenericRepository<OtpCode>(_dbContext);

        public IGenericRepository<RefreshToken> RefreshTokens => 
            _refreshTokens ??= new GenericRepository<RefreshToken>(_dbContext);

        public IGenericRepository<LoginAttempt> LoginAttempts => 
            _loginAttempts ??= new GenericRepository<LoginAttempt>(_dbContext);

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
