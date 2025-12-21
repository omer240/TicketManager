using TicketManager.Api.Domain.Entities;

namespace TicketManager.Api.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<IReadOnlyList<Comment>> GetByTicketIdAsync(int ticketId,CancellationToken ct = default);

        Task<Comment?> GetByIdAsync(int id,bool asNoTracking = true,CancellationToken ct = default);

        Task<IReadOnlyList<Comment>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    }
}
