using Microsoft.AspNetCore.Identity;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Paging;
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

        public async Task<PagedResult<TicketDto>> GetMyCreatedAsync(string userId, TicketQuery query, CancellationToken ct = default)
        {
            var result = await _ticketRepo.GetCreatedPagedAsync(userId, query, ct);

            return new PagedResult<TicketDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
           
        public async Task<PagedResult<TicketDto>> GetMyAssignedAsync(string userId, TicketQuery query, CancellationToken ct = default)
        {
            var result = await _ticketRepo.GetAssignedPagedAsync(userId, query, ct);

            return new PagedResult<TicketDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<TicketDto> CreateAsync(string userId, TicketCreateRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw ApiException.BadRequest("Title zorunludur.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw ApiException.BadRequest("Description zorunludur.");

            if (string.IsNullOrWhiteSpace(request.AssignedToUserId))
                throw ApiException.BadRequest("Ticket mutlaka bir kullanıcıya atanmalıdır.");

            if (request.AssignedToUserId == userId)
                throw ApiException.BadRequest("Ticket oluşturan kişi ticket'ı kendisine atayamaz.");

            var creator = await _userManager.FindByIdAsync(userId);
            if (creator is null)
                throw ApiException.BadRequest("Kullanıcı bulunamadı.");

            var assignee = await _userManager.FindByIdAsync(request.AssignedToUserId);
            if (assignee is null)
                throw ApiException.BadRequest("Atanacak kullanıcı bulunamadı.");

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

            return ToDto(ticket);

        }

        public async Task<TicketDto> UpdateAsync(string userId, int ticketId, TicketUpdateRequest request, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.GetByIdWithUsersAsync(ticketId, ct);

            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureCreator(userId, ticket);

            if (string.IsNullOrWhiteSpace(request.Title))
                throw ApiException.BadRequest("Title zorunludur.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw ApiException.BadRequest("Description zorunludur.");

            ticket.Title = request.Title.Trim();
            ticket.Description = request.Description.Trim();
            ticket.Priority = request.Priority;
            ticket.UpdatedAt = DateTimeOffset.UtcNow;

            _ticketRepo.Update(ticket);
            await _uow.SaveChangesAsync(ct);


            return ToDto(ticket);
        }

        public async Task<TicketDto> UpdateStatusAsync(string userId, int ticketId, TicketStatus status, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.GetByIdWithUsersAsync(ticketId, ct);

            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureAssignee(userId, ticket);

            ticket.Status = status;
            ticket.UpdatedAt = DateTimeOffset.UtcNow;

            _ticketRepo.Update(ticket);
            await _uow.SaveChangesAsync(ct);

            return ToDto(ticket);
        }
        public async Task<TicketDto> GetDetailAsync(string userId, int ticketId, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.GetDetailAsync(ticketId, userId, ct);
            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            return ToDto(ticket);
        }

        public async Task DeleteAsync(string userId, int ticketId, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: false, ct: ct);
            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureCreator(userId, ticket);

            _ticketRepo.Remove(ticket);
            await _uow.SaveChangesAsync(ct);
        }


        private void EnsureCreator(string userId, Ticket ticket)
        {
            if (ticket.CreatedByUserId != userId)
                throw ApiException.Forbidden("Bu ticket üzerinde genel güncelleme yetkin yok. Sadece oluşturan kişi güncelleyebilir.");
        }

        private void EnsureAssignee(string userId, Ticket ticket)
        {
            if (ticket.AssignedToUserId != userId)
                throw ApiException.Forbidden("Bu ticket'in durumunu sadece atanmış kişi güncelleyebilir.");
        }

        private TicketDto ToDto(Ticket t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            CreatedByUserId = t.CreatedByUserId,
            CreatedByUserFullName = t.CreatedByUser?.FullName,
            AssignedToUserId = t.AssignedToUserId,
            AssignedToUserFullName = t.AssignedToUser?.FullName,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            CommentCount = t.Comments?.Count ?? 0
        };
    }
}
