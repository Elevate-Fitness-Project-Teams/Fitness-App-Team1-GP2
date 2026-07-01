using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorkoutService.Contracts;
using WorkoutService.Database;

namespace WorkoutService.Persistence;

public class GenericRepository<TEntity>(WorkoutDbContext context)
    : IGenericRepository<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetAll(bool asNoTracking = true)
    {
        var query = context.Set<TEntity>().AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = context.Set<TEntity>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>()
            .AnyAsync(predicate, cancellationToken);
    }
}