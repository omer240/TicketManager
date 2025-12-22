using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManager.Api.ApiModels.Comments;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Responses;
using TicketManager.Api.Services.Interfaces;

namespace TicketManager.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET api/tickets/{ticketId}/comments
        [HttpGet("tickets/{ticketId:int}/comments")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CommentDto>>>> ByTicket(int ticketId,CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var list = await _commentService.GetByTicketIdAsync(userId, ticketId, ct);
            return Ok(ApiResponse<IReadOnlyList<CommentDto>>.Ok(list));
        }

        // POST api/tickets/{ticketId}/comments
        [HttpPost("tickets/{ticketId:int}/comments")]
        public async Task<ActionResult<ApiResponse<CommentDto>>> Create(int ticketId,[FromBody] CommentCreateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();

            var dto = await _commentService.AddToTicketAsync(
                userId,
                ticketId,
                new CommentCreateRequest { Text = request.Text },
                ct);

            return Ok(ApiResponse<CommentDto>.Ok(dto));
        }

        // PUT api/comments/{commentId}
        [HttpPut("comments/{commentId:int}")]
        public async Task<ActionResult<ApiResponse<CommentDto>>> Update(int commentId,[FromBody] CommentUpdateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _commentService.UpdateAsync(userId, commentId, request, ct);
            return Ok(ApiResponse<CommentDto>.Ok(dto));
        }

        // DELETE api/comments/{commentId}
        [HttpDelete("comments/{commentId:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int commentId,CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            await _commentService.DeleteAsync(userId, commentId, ct);
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        private string GetUserIdOrThrow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw ApiException.Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            return userId;
        }
    }
}