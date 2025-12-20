using Microsoft.AspNetCore.Identity;
using TicketManager.Api.ApiModels.Comments;
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

        public async Task<IReadOnlyList<Comment>> GetByTicketIdAsync(string userId, int ticketId, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            var canView =
                ticket.CreatedByUserId == userId ||
                ticket.AssignedToUserId == userId;

            if (!canView)
                throw new UnauthorizedAccessException("Bu ticket'ın yorumlarını görme yetkin yok.");

            return await _commentRepo.GetByTicketIdAsync(ticketId, ct);
        }

        public async Task<Comment> AddToTicketAsync(string userId, int ticketId, CommentCreateRequest request, CancellationToken ct = default)
        {
            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == ticketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            var canComment =
                ticket.CreatedByUserId == userId ||
                ticket.AssignedToUserId == userId;

            if (!canComment)
                throw new UnauthorizedAccessException("Bu ticket'a yorum ekleme yetkin yok.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new InvalidOperationException("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(request.Text))
                throw new InvalidOperationException("Yorum metni boş olamaz.");

            var comment = new Comment
            {
                TicketId = ticketId,
                Text = request.Text.Trim(),
                CreatedByUserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _commentRepo.AddAsync(comment, ct);
            await _uow.SaveChangesAsync(ct);

            return comment;
        }

        public async Task<Comment> UpdateAsync(string userId,int commentId,CommentUpdateRequest request,CancellationToken ct = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, asNoTracking: false, ct: ct);
            if (comment is null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            if (comment.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("Bu yorumu güncelleme yetkin yok.");

            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == comment.TicketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            var isRelated =
                ticket.CreatedByUserId == userId ||
                ticket.AssignedToUserId == userId;

            if (!isRelated)
                throw new UnauthorizedAccessException("Bu ticket üzerinde işlem yetkin yok.");

            if (string.IsNullOrWhiteSpace(request.Text))
                throw new InvalidOperationException("Yorum metni boş olamaz.");

            comment.Text = request.Text.Trim();

            _commentRepo.Update(comment);
            await _uow.SaveChangesAsync(ct);

            return comment;
        }

        public async Task DeleteAsync(string userId, int commentId, CancellationToken ct = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, asNoTracking: false, ct: ct);
            if (comment is null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            if (comment.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("Bu yorumu silme yetkin yok.");

            var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == comment.TicketId, asNoTracking: true, ct: ct);
            if (ticket is null)
                throw new KeyNotFoundException("Ticket bulunamadı.");

            var isRelated =
                ticket.CreatedByUserId == userId ||
                ticket.AssignedToUserId == userId;

            if (!isRelated)
                throw new UnauthorizedAccessException("Bu ticket üzerinde işlem yetkin yok.");

            _commentRepo.Remove(comment);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
