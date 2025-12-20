using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TicketManager.Api.Data.Contexts;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IReadOnlyList<Comment>> GetByTicketIdAsync(int ticketId, CancellationToken ct = default)
        {
            return await _set
                .AsNoTracking()
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Comment?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken ct = default)
        {
            IQueryable<Comment> query = _set;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<IReadOnlyList<Comment>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        {
            return await _set
                .AsNoTracking()
                .Where(c => c.CreatedByUserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
