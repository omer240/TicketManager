using Microsoft.EntityFrameworkCore;
using TicketManager.Api.ApiModels.Common;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Domain.Enums;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(DbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Ticket>> GetPagedAsync(TicketQuery query, CancellationToken ct = default)
        {
            if (query.Page <= 0) query = query with { Page = 1 };
            if (query.PageSize <= 0) query = query with { PageSize = 20 };

            var q = _set
                .AsNoTracking()
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(t =>
                    t.Title.Contains(s) ||
                    t.Description.Contains(s));
            }

            if (query.Status.HasValue)
                q = q.Where(t => t.Status == query.Status.Value);

            if (query.Priority.HasValue)
                q = q.Where(t => t.Priority == query.Priority.Value);

            if (!string.IsNullOrWhiteSpace(query.AssignedToUserId))
                q = q.Where(t => t.AssignedToUserId == query.AssignedToUserId);

            if (!string.IsNullOrWhiteSpace(query.CreatedByUserId))
                q = q.Where(t => t.CreatedByUserId == query.CreatedByUserId);


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

        public Task<Ticket?> GetDetailAsync(int id, CancellationToken ct = default)
        {
            return _set
                .AsNoTracking()
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<bool> UpdateStatusAsync(int id, TicketStatus newStatus, DateTimeOffset nowUtc, CancellationToken ct = default)
        {
            var ticket = await _set.FirstOrDefaultAsync(t => t.Id == id, ct);
            if (ticket is null) return false;

            ticket.Status = newStatus;
            ticket.UpdatedAt = nowUtc;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> AssignToAsync(int id, string? assignedToUserId, DateTimeOffset nowUtc, CancellationToken ct = default)
        {
            var ticket = await _set.FirstOrDefaultAsync(t => t.Id == id, ct);
            if (ticket is null) return false;

            ticket.AssignedToUserId = assignedToUserId; 
            ticket.UpdatedAt = nowUtc;

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
