using TicketManager.Api.ApiModels.Comments;
using TicketManager.Api.Domain.Entities;

namespace TicketManager.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IReadOnlyList<CommentDto>> GetByTicketIdAsync(string userId,int ticketId,CancellationToken ct = default);

        Task<CommentDto> AddToTicketAsync(string userId,int ticketId,CommentCreateRequest request,CancellationToken ct = default);

        Task<CommentDto> UpdateAsync(string userId, int commentId, CommentUpdateRequest request, CancellationToken ct = default);

        Task DeleteAsync(string userId,int commentId,CancellationToken ct = default);
    }
}
