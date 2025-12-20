using Microsoft.AspNetCore.Identity;
using TicketManager.Api.ApiModels.Common;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Domain.Enums;
using TicketManager.Api.Repositories.Interfaces;
using TicketManager.Api.Services.Interfaces;

namespace TicketManager.Api.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketService(
            ITicketRepository ticketRepo,
            IUnitOfWork uow,
            UserManager<ApplicationUser> userManager)
        {
            _ticketRepo = ticketRepo;
            _uow = uow;
            _userManager = userManager;
        }

        public Task<PagedResult<Ticket>> GetMyCreatedAsync(string userId, TicketQuery query, CancellationToken ct = default)
            => _ticketRepo.GetCreatedPagedAsync(userId, query, ct);

        public Task<PagedResult<Ticket>> GetMyAssignedAsync(string userId, TicketQuery query, CancellationToken ct = default)
            => _ticketRepo.GetAssignedPagedAsync(userId, query, ct);

        public async Task<Ticket> CreateAsync(string userId, TicketCreateRequest request, CancellationToken ct = default)
        {
            var creator = await _userManager.FindByIdAsync(userId);
            if (creator is null)
                throw new InvalidOperationException("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(request.AssignedToUserId))
                throw new InvalidOperationException("Ticket mutlaka bir kullanıcıya atanmalıdır.");

            var assignee = await _userManager.FindByIdAsync(request.AssignedToUserId);
            if (assignee is null)
                throw new InvalidOperationException("Atanacak kullanıcı bulunamadı.");

            var now = DateTimeOffset.UtcNow;

            var ticket = new Ticket
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Priority = request.Priority,
                Status = TicketStatus.Open,

                CreatedByUserId = userId,
                AssignedToUserId = request.AssignedToUserId,

                CreatedAt = now,
                UpdatedAt = now
            };

            await _ticketRepo.AddAsync(ticket, ct);
            await _uow.SaveChangesAsync(ct);

            return ticket;
        
    }

        public async Task<Ticket> UpdateAsync(string userId, int ticketId, TicketUpdateRequest request, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: false, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            if (ticket.CreatedByUserId != userId && ticket.AssignedToUserId != userId)
                throw new UnauthorizedAccessException("Bu ticket üzerinde işlem yetkin yok.");

            ticket.Title = request.Title.Trim();
            ticket.Description = request.Description.Trim();
            ticket.Status = request.Status;
            ticket.Priority = request.Priority;
            ticket.UpdatedAt = DateTimeOffset.UtcNow;

            _ticketRepo.Update(ticket);

            await _uow.SaveChangesAsync(ct);
            return ticket;
        }

        public async Task<Ticket> UpdateStatusAsync(string userId, int ticketId, TicketStatus status, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: false, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            if (ticket.CreatedByUserId != userId && ticket.AssignedToUserId != userId)
                throw new UnauthorizedAccessException("Bu ticket üzerinde işlem yetkin yok.");

            ticket.Status = status;
            ticket.UpdatedAt = DateTimeOffset.UtcNow;

            _ticketRepo.Update(ticket);
            await _uow.SaveChangesAsync(ct);

            return ticket;
        }

        public async Task<Ticket> AssignAsync(string userId, int ticketId, string? assignedToUserId, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: false, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            if (ticket.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("Ticket atamasını sadece oluşturan kullanıcı değiştirebilir.");

            if (!string.IsNullOrWhiteSpace(assignedToUserId))
            {
                var assignee = await _userManager.FindByIdAsync(assignedToUserId);
                if (assignee is null)
                    throw new InvalidOperationException("Atanacak kullanıcı bulunamadı.");

                ticket.AssignedToUserId = assignedToUserId;
            }
            else
            {

                ticket.AssignedToUserId = null;
            }

            ticket.UpdatedAt = DateTimeOffset.UtcNow;

            _ticketRepo.Update(ticket);
            await _uow.SaveChangesAsync(ct);

            return ticket;
        }

        public async Task<Ticket> GetDetailAsync(int ticketId, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.GetDetailAsync(ticketId, ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            return ticket;
        }
    }
}
