using System.Linq.Expressions;

namespace FCEService.Common.Abstract
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T?> GetById(int id, params Expression<Func<T, object>>[] includes);
        IQueryable<T> Get(Expression<Func<T, bool>> expression);
        Task SaveChangeAsync(CancellationToken cancellationToken);
        void Add(T entity);
        void Update(T entity, params string[] modifiedParams);
        void SoftDelete(int id);
    }
}
