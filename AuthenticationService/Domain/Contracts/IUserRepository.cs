using AuthenticationService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
    }
}
