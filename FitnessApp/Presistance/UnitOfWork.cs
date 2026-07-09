using System.Collections.Concurrent;
using WorkoutService.Contracts;
using WorkoutService.Database;

namespace WorkoutService.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly WorkoutDbContext _context;
    private readonly ConcurrentDictionary<string, object> _repositories;

    public UnitOfWork(WorkoutDbContext context)
    {
        _context = context;
        _repositories = new ConcurrentDictionary<string, object>();
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class
    {
        var typeName = typeof(TEntity).Name; 
        
        return (IGenericRepository<TEntity>)_repositories.GetOrAdd(  
            typeName,
            _ => new GenericRepository<TEntity>(_context)
        );
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}