using FCEService.Common.Abstract;
using FCEService.Domain.Entities;
using FCEService.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
namespace FCEService.Infrastructure.Persistance
{
    public class GenericRepository<T>(FCE_DBContext _context) : IGenericRepository<T> where T : BaseEntity
    {

        public IQueryable<T> GetAll()
        {
            var result = _context.Set<T>().AsQueryable();
            return result;
        }

        public IQueryable<T?> GetById(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().Where(x => x.Id == id);
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }
        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        public async Task SaveChangeAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T entity, params string[] modifiedParams)
        {
            var local = _context.Set<T>().Local.FirstOrDefault(x => x.Id == entity.Id);

            EntityEntry entry;

            if (local == null)
            {
                _context.Attach(entity);
                entry = _context.Entry(entity);
            }
            else
            {
                entry = _context.Entry(local);
            }

            foreach (var propName in modifiedParams)
            {
                var property = entry.Metadata.FindProperty(propName);

                if (property == null)
                {
                    throw new InvalidOperationException(
                        $"'{propName}' is not a mapped scalar property of {typeof(T).Name}.");
                }

                var value = typeof(T).GetProperty(propName)!.GetValue(entity);

                entry.Property(propName).CurrentValue = value;
                entry.Property(propName).IsModified = true;
            }
        }

        public void SoftDelete(int id)
        {
            var local = _context.Set<T>().Local
                .FirstOrDefault(x => x.Id == id);

            EntityEntry entry;
            if (local == null)
            {
                var entity = Activator.CreateInstance<T>(); // Create a new instance of T 
                entity.Id = id;

                _context.Set<T>().Attach(entity);
                entry = _context.Entry(entity);
            }
            else
            {
                entry = _context.Entry(local);
            }
            entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
            entry.Property(nameof(BaseEntity.IsDeleted)).IsModified = true;

        }
    }
}
