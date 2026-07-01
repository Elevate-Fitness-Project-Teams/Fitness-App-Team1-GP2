namespace WorkoutService.Contracts;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}