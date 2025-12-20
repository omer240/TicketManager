using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using TicketManager.Api.ApiModels.Comments;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Repositories.Interfaces;
using TicketManager.Api.Services.Interfaces;

namespace TicketManager.Api.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentService(
            ICommentRepository commentRepo,
            ITicketRepository ticketRepo,
            IUnitOfWork uow,
            UserManager<ApplicationUser> userManager)
        {
            _commentRepo = commentRepo;
            _ticketRepo = ticketRepo;
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<CommentDto>> GetByTicketIdAsync(string userId, int ticketId, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureTicketRelated(userId, ticket);

            var comments = await _commentRepo.GetByTicketIdAsync(ticketId, ct);

            return comments.Select(ToDto).ToList();
        }

        public async Task<CommentDto> AddToTicketAsync(string userId, int ticketId, CommentCreateRequest request, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureTicketRelated(userId, ticket);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw ApiException.BadRequest("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(request.Text))
                throw ApiException.BadRequest("Yorum metni boş olamaz.");

            var comment = new Comment
            {
                TicketId = ticketId,
                Text = request.Text.Trim(),
                CreatedByUserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _commentRepo.AddAsync(comment, ct);
            await _uow.SaveChangesAsync(ct);

            return ToDto(comment);
        }

        public async Task<CommentDto> UpdateAsync(string userId,int commentId,CommentUpdateRequest request,CancellationToken ct = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, asNoTracking: false, ct: ct);
            if (comment is null)
                throw ApiException.NotFound("Yorum bulunamadı.");

            if (comment.CreatedByUserId != userId)
                throw ApiException.Forbidden("Bu yorumu güncelleme yetkin yok.");

            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == comment.TicketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureTicketRelated(userId, ticket);

            if (string.IsNullOrWhiteSpace(request.Text))
                throw ApiException.BadRequest("Yorum metni boş olamaz.");

            comment.Text = request.Text.Trim();

            _commentRepo.Update(comment);
            await _uow.SaveChangesAsync(ct);

            return ToDto(comment);
        }

        public async Task DeleteAsync(string userId, int commentId, CancellationToken ct = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, asNoTracking: false, ct: ct);
            if (comment is null)
                throw ApiException.NotFound("Yorum bulunamadı.");

            if (comment.CreatedByUserId != userId)
                throw ApiException.Forbidden("Bu yorumu silme yetkin yok.");

            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == comment.TicketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw ApiException.NotFound("Ticket bulunamadı.");

            EnsureTicketRelated(userId, ticket);

            _commentRepo.Remove(comment);
            await _uow.SaveChangesAsync(ct);
        }


        private  void EnsureTicketRelated(string userId, Ticket ticket)
        {
            var isRelated =
                ticket.CreatedByUserId == userId ||
                ticket.AssignedToUserId == userId;

            if (!isRelated)
                throw ApiException.Forbidden("Bu ticket üzerinde işlem yetkin yok.");
        }

        private CommentDto ToDto(Comment c) => new()
        {
            Id = c.Id,
            TicketId = c.TicketId,
            Text = c.Text,
            CreatedByUserId = c.CreatedByUserId,
            CreatedAt = c.CreatedAt
        };
    }
}
