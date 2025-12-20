using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]/[action]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET api/comments/ByTicket?ticketId=5
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CommentDto>>>> ByTicket(
            [FromQuery] int ticketId,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var list = await _commentService.GetByTicketIdAsync(userId, ticketId, ct);
            return Ok(ApiResponse<IReadOnlyList<CommentDto>>.Ok(list));
        }

        public sealed class AddCommentRequest
        {
            public int TicketId { get; set; }
            public string Text { get; set; } = default!;
        }

        // POST api/comments/Add
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CommentDto>>> Add(
            [FromBody] AddCommentRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();

            var dto = await _commentService.AddToTicketAsync(
                userId,
                request.TicketId,
                new CommentCreateRequest { Text = request.Text },
                ct);

            return Ok(ApiResponse<CommentDto>.Ok(dto));
        }

        // PUT api/comments/Update?commentId=10
        [HttpPut]
        public async Task<ActionResult<ApiResponse<CommentDto>>> Update(
            [FromQuery] int commentId,
            [FromBody] CommentUpdateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _commentService.UpdateAsync(userId, commentId, request, ct);
            return Ok(ApiResponse<CommentDto>.Ok(dto));
        }

        // DELETE api/comments/Delete?commentId=10
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> Delete(
            [FromQuery] int commentId,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            await _commentService.DeleteAsync(userId, commentId, ct);
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        private string GetUserIdOrThrow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw ApiException.Unauthorized("User not authenticated.");
            return userId;
        }

    }
}
