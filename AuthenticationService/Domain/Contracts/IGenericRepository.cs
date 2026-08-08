using AuthenticationService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Contracts
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        IQueryable<T> GetQueryable(bool ignoreQueryFilters = false);
    }
}
