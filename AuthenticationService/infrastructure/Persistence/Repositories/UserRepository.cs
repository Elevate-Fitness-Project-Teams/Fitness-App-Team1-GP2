using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using AuthenticationService.infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AuthDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User?> GetByEmailAsync(string email, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            var query = GetQueryable(ignoreQueryFilters);
            return await query.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
