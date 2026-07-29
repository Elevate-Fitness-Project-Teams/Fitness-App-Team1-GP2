using Microsoft.EntityFrameworkCore;
using NutritionService.Domain.Common.Interfaces;

namespace NutritionService.Infrastructure.Persistence;

public sealed class Repository<T> : IRepository<T> where T : class
{
    private readonly NutritionDbContext _db;
    private readonly DbSet<T> _set;

    public Repository(NutritionDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _set.FindAsync(new object[] { id }, ct);

    public IQueryable<T> Query() => _set.AsQueryable();

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await _set.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) =>
        await _set.AddRangeAsync(entities, ct);

    public void Update(T entity) => _set.Update(entity);

    public void Delete(T entity) => _set.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
}
