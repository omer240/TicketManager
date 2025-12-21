using TicketManager.Api.ApiModels.Common.Paging;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Domain.Entities;

namespace TicketManager.Api.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<PagedResult<Ticket>> GetCreatedPagedAsync(string userId,TicketQuery query,CancellationToken ct = default);

        Task<PagedResult<Ticket>> GetAssignedPagedAsync(string userId,TicketQuery query,CancellationToken ct = default);

        Task<Ticket?> GetDetailAsync(int ticketId, string userId, CancellationToken ct = default);

        
    }
}
