using Microsoft.EntityFrameworkCore.Storage;
using NutritionService.Domain.Common.Interfaces;

namespace NutritionService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly NutritionDbContext _db;
    private readonly Dictionary<Type, object> _repos = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(NutritionDbContext db) => _db = db;

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repos.TryGetValue(type, out var repo))
        {
            repo = new Repository<T>(_db);
            _repos[type] = repo;
        }
        return (IRepository<T>)repo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        _transaction = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await _db.SaveChangesAsync(ct);
            await _transaction.CommitAsync(ct);
        }
        catch
        {
            await _transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();
        await _db.DisposeAsync();
    }
}
