using AuthenticationService.Domain.Contracts;
using AuthenticationService.Domain.Entities;
using AuthenticationService.infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AuthDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            await UpdateAsync(entity, cancellationToken);
        }

        public IQueryable<T> GetQueryable(bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            return query;
        }
    }
}
