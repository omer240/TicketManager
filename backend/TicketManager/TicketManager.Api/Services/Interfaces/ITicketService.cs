using TicketManager.Api.ApiModels.Common;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.Services.Interfaces
{
    public interface ITicketService
    {
        Task<PagedResult<Ticket>> GetMyCreatedAsync(string userId, TicketQuery query, CancellationToken ct = default);
        Task<PagedResult<Ticket>> GetMyAssignedAsync(string userId, TicketQuery query, CancellationToken ct = default);

        Task<Ticket> CreateAsync(string userId, TicketCreateRequest request, CancellationToken ct = default);
        Task<Ticket> UpdateAsync(string userId, int ticketId, TicketUpdateRequest request, CancellationToken ct = default);

        Task<Ticket> UpdateStatusAsync(string userId, int ticketId, TicketStatus status, CancellationToken ct = default);
        Task<Ticket> AssignAsync(string userId, int ticketId, string? assignedToUserId, CancellationToken ct = default);

        Task<Ticket> GetDetailAsync(int ticketId, CancellationToken ct = default);
    }
}
