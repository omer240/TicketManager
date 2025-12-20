using TicketManager.Api.ApiModels.Common.Paging;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.Services.Interfaces
{
    public interface ITicketService
    {
        Task<PagedResult<TicketDto>> GetMyCreatedAsync(string userId, TicketQuery query, CancellationToken ct = default);
        Task<PagedResult<TicketDto>> GetMyAssignedAsync(string userId, TicketQuery query, CancellationToken ct = default);

        Task<TicketDto> CreateAsync(string userId, TicketCreateRequest request, CancellationToken ct = default);
        Task<TicketDto> UpdateAsync(string userId, int ticketId, TicketUpdateRequest request, CancellationToken ct = default);

        Task<TicketDto> UpdateStatusAsync(string userId, int ticketId, TicketStatus status, CancellationToken ct = default);
        Task<TicketDto> GetDetailAsync(string userId, int ticketId, CancellationToken ct = default);
    }
}
