using Microsoft.EntityFrameworkCore;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(DbContext context) : base(context) { }

        public async Task<IReadOnlyList<Comment>> GetByTicketIdAsync(int ticketId, CancellationToken ct = default)
        {
            return await _set.AsNoTracking()
                .Where(c => c.TicketId == ticketId)
                .Include(c => c.CreatedByUser)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
