using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Paging;
using TicketManager.Api.ApiModels.Common.Responses;
using TicketManager.Api.ApiModels.Tickets;
using TicketManager.Api.Services.Interfaces;

namespace TicketManager.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET api/tickets/created?page=1&pageSize=20&search=...&status=...&priority=...
        [HttpGet("created")]
        public async Task<ActionResult<ApiResponse<PagedResult<TicketDto>>>> MyCreated([FromQuery] TicketQuery query,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var result = await _ticketService.GetMyCreatedAsync(userId, query, ct);
            return Ok(ApiResponse<PagedResult<TicketDto>>.Ok(result));
        }

        // GET api/tickets/assigned?page=1&pageSize=20&search=...&status=...&priority=...
        [HttpGet("assigned")]
        public async Task<ActionResult<ApiResponse<PagedResult<TicketDto>>>> MyAssigned([FromQuery] TicketQuery query,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var result = await _ticketService.GetMyAssignedAsync(userId, query, ct);
            return Ok(ApiResponse<PagedResult<TicketDto>>.Ok(result));
        }

        // GET api/tickets/{ticketId}
        [HttpGet("{ticketId:int}")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> Detail(int ticketId,CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.GetDetailAsync(userId, ticketId, ct);
            return Ok(ApiResponse<TicketDto>.Ok(dto));
        }

        // POST api/tickets
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TicketDto>>> Create([FromBody] TicketCreateRequest request,CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.CreateAsync(userId, request, ct);

            return CreatedAtAction(nameof(Detail), new { ticketId = dto.Id }, ApiResponse<TicketDto>.Ok(dto));
        }

        // PUT api/tickets/{ticketId}
        [HttpPut("{ticketId:int}")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> Update(int ticketId,[FromBody] TicketUpdateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.UpdateAsync(userId, ticketId, request, ct);
            return Ok(ApiResponse<TicketDto>.Ok(dto));
        }

        // DELETE api/tickets/{ticketId}
        [HttpDelete("{ticketId:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int ticketId,CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            await _ticketService.DeleteAsync(userId, ticketId, ct);
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        // PATCH api/tickets/{ticketId}/status
        [HttpPatch("{ticketId:int}/status")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> UpdateStatus(int ticketId,[FromBody] TicketStatusUpdateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.UpdateStatusAsync(userId, ticketId, request.Status, ct);
            return Ok(ApiResponse<TicketDto>.Ok(dto));
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