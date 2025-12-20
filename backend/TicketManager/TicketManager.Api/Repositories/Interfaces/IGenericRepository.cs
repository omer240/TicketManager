using System.Linq.Expressions;

namespace TicketManager.Api.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,bool asNoTracking = true,CancellationToken ct = default);
    }
}
