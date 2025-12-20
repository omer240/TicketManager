using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TicketManager.Api.ApiModels.Auth;
using TicketManager.Api.ApiModels.Common.Responses;
using TicketManager.Api.Extensions;
using TicketManager.Api.Services.Interfaces.Auth;

namespace TicketManager.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableRateLimiting(RateLimitExtensions.AuthPolicy)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
            [FromBody] RegisterRequest request,
            CancellationToken ct)
        {
            var dto = await _authService.RegisterAsync(request, ct);
            return Ok(ApiResponse<AuthResponse>.Ok(dto));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
            [FromBody] LoginRequest request,
            CancellationToken ct)
        {
            var dto = await _authService.LoginAsync(request, ct);
            return Ok(ApiResponse<AuthResponse>.Ok(dto));
        }
    }
}
