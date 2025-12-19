using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _set;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public IQueryable<T> Query(bool asNoTracking = true)
            => asNoTracking ? _set.AsNoTracking() : _set;

        public async Task<T?> GetByIdAsync(object id, bool asNoTracking = true, CancellationToken ct = default)
        {
            // FindAsync tracking döndürür, o yüzden burada "asNoTracking" için küçük workaround:
            var entity = await _set.FindAsync(new[] { id }, ct);
            if (entity is null) return null;

            if (asNoTracking)
                _context.Entry(entity).State = EntityState.Detached;

            return entity;
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken ct = default)
            => asNoTracking
                ? await _set.AsNoTracking().FirstOrDefaultAsync(predicate, ct)
                : await _set.FirstOrDefaultAsync(predicate, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await _set.AddAsync(entity, ct);

        public void Update(T entity) => _set.Update(entity);

        public void Remove(T entity) => _set.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);
    }
}
