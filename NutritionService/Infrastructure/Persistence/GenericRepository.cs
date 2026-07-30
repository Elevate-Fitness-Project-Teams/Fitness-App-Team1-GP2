using Microsoft.EntityFrameworkCore;
using NutritionService.Common.Abstractions;
using System.Linq.Expressions;

namespace NutritionService.Infrastructure.Persistence;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly NutritionDbContext Context;
    protected readonly DbSet<TEntity> Set;

    public GenericRepository(NutritionDbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Set.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking().ToListAsync(cancellationToken);

    public IQueryable<TEntity> Query(bool asNoTracking = true) =>
        asNoTracking ? Set.AsNoTracking() : Set;

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? await Set.CountAsync(cancellationToken)
            : await Set.CountAsync(predicate, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await Set.AddAsync(entity, cancellationToken);

    public void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Set.Update(entity);
    }

    public void Remove(TEntity entity) => Set.Remove(entity);
}
