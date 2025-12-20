using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketManager.Api.Data.Contexts;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        protected readonly DbSet<T> _set;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
           await  _set.AddAsync(entity, ct).AsTask();
        }

        public void Update(T entity)
        {
            _set.Update(entity);
        }
            
        public void Remove(T entity)
        {
            _set.Remove(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _set.AnyAsync(predicate, ct);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,bool asNoTracking = true,CancellationToken ct = default)
        {
            IQueryable<T> query = _set;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(predicate, ct);
        }

    }
}
