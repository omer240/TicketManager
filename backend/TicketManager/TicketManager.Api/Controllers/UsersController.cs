using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Responses;
using TicketManager.Api.ApiModels.Users;
using TicketManager.Api.Services.Interfaces;

namespace TicketManager.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/users/Assignees?search=mehmet
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> Assignees(
            [FromQuery] string? search,
            CancellationToken ct)
        {
            var userId = GetUserIdOrThrow();
            var users = await _userService.GetAssigneesAsync(userId, search, ct);
            return Ok(ApiResponse<IReadOnlyList<UserDto>>.Ok(users));
        }

        //JWT içindeki NameIdentifier claimden giriş yapan kullanıcı bilgilerini okumak için kullanılan yardımcı metot
        private string GetUserIdOrThrow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw ApiException.Unauthorized("Kullanıcı kimliği doğrulanamadı.");

            return userId;
        }
    }
}
