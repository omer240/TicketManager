using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]/[action]")]

    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET api/tickets/MyCreated?page=1&pageSize=20&search=...&status=...&priority=...
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TicketDto>>>> MyCreated([FromQuery] TicketQuery query,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var result = await _ticketService.GetMyCreatedAsync(userId, query, ct);
            return Ok(ApiResponse<PagedResult<TicketDto>>.Ok(result));
        }

        // GET api/tickets/MyAssigned?page=1&pageSize=20&search=...&status=...&priority=...
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TicketDto>>>> MyAssigned([FromQuery] TicketQuery query,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var result = await _ticketService.GetMyAssignedAsync(userId, query, ct);
            return Ok(ApiResponse<PagedResult<TicketDto>>.Ok(result));
        }

        // GET api/tickets/Detail?ticketId=5
        [HttpGet]
        public async Task<ActionResult<ApiResponse<TicketDto>>> Detail([FromQuery] int ticketId,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.GetDetailAsync(userId, ticketId, ct);
            return Ok(ApiResponse<TicketDto>.Ok(dto));
        }

        // PUT api/tickets/Update?ticketId=5
        [HttpPut]
        public async Task<ActionResult<ApiResponse<TicketDto>>> Update([FromQuery] int ticketId,[FromBody] TicketUpdateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.UpdateAsync(userId, ticketId, request, ct);
            return Ok(ApiResponse<TicketDto>.Ok(dto));
        }


        // POST api/tickets/Create
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TicketDto>>> Create([FromBody] TicketCreateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.CreateAsync(userId, request, ct);

            return CreatedAtAction(nameof(Detail), new { ticketId = dto.Id }, ApiResponse<TicketDto>.Ok(dto));
        }


        // POST api/tickets/Delete?ticketId=5
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] int ticketId, 
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            await _ticketService.DeleteAsync(userId, ticketId, ct);
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        // PATCH api/tickets/UpdateStatus
        [HttpPatch]
        public async Task<ActionResult<ApiResponse<TicketDto>>> UpdateStatus([FromBody] TicketStatusUpdateRequest request,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var dto = await _ticketService.UpdateStatusAsync(userId, request.TicketId, request.Status, ct);
            return Ok(ApiResponse<TicketDto>.Ok(dto));
        }

        //JWT içindeki NameIdentifier claimden giriş yapan kullanıcı bilgilerini okumak için kullanılan yardımcı metot
        private string GetUserIdOrThrow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw ApiException.Unauthorized("User not authenticated.");
            return userId;
        }
    }


}
