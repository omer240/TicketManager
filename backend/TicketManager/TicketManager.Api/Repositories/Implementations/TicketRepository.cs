using Microsoft.EntityFrameworkCore;
using TicketManager.Api.ApiModels.Common.Paging;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Data.Contexts;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Ticket>> GetCreatedPagedAsync(
            string userId,
            TicketQuery query,
            CancellationToken ct = default)
        {
            query = NormalizeQuery(query);

            var q = BuildBaseQuery(query)
                .Where(t => t.CreatedByUserId == userId);

            return await ToPagedResultAsync(q, query, ct);
        }

        public async Task<PagedResult<Ticket>> GetAssignedPagedAsync(
            string userId,
            TicketQuery query,
            CancellationToken ct = default)
        {
            query = NormalizeQuery(query);

            var q = BuildBaseQuery(query)
                .Where(t => t.AssignedToUserId == userId);

            return await ToPagedResultAsync(q, query, ct);
        }

        public async  Task<Ticket?> GetDetailAsync(int ticketId, string userId, CancellationToken ct = default)
        {
            return await _set
             .AsNoTracking()
             .Include(t => t.CreatedByUser)
             .Include(t => t.AssignedToUser)
             .Include(t => t.Comments)
                 .ThenInclude(c => c.CreatedByUser)
             .FirstOrDefaultAsync(t =>
                 t.Id == ticketId &&
                 (t.CreatedByUserId == userId || t.AssignedToUserId == userId),
                 ct);
        }

        public async Task<Ticket?> GetByIdWithUsersAsync(int ticketId, CancellationToken ct = default)
        {
            return await _set
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == ticketId, ct);
        }


        private IQueryable<Ticket> BuildBaseQuery(TicketQuery query)
        {
            var q = _set
                .AsNoTracking()
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(t => t.Title.Contains(s) || t.Description.Contains(s));
            }

            if (query.Status.HasValue)
                q = q.Where(t => t.Status == query.Status.Value);

            if (query.Priority.HasValue)
                q = q.Where(t => t.Priority == query.Priority.Value);

            return q;
        }

        private static TicketQuery NormalizeQuery(TicketQuery query)
        {
            int page;
            int pageSize;

            if (query.Page <= 0)
            {
                page = 1;
            }
            else
            {
                page = query.Page;
            }

            if (query.PageSize <= 0)
            {
                pageSize = 20;
            }
            else
            {
                pageSize = query.PageSize;
            }

            return query with
            {
                Page = page,
                PageSize = pageSize
            };
        }

        private static async Task<PagedResult<Ticket>> ToPagedResultAsync(
            IQueryable<Ticket> q,
            TicketQuery query,
            CancellationToken ct)
        {
            var total = await q.CountAsync(ct);

            q = q.OrderByDescending(t => t.UpdatedAt);

            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Ticket>
            {
                Items = items,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
