using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TicketManager.Api.ApiModels.Auth.Login;
using TicketManager.Api.ApiModels.Auth.Register;
using TicketManager.Api.ApiModels.Common.Responses;
using TicketManager.Api.Extensions;
using TicketManager.Api.Services.Interfaces.Auth;

namespace TicketManager.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting(RateLimitExtensions.AuthPolicy)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request,CancellationToken ct)
        {
            var dto = await _authService.RegisterAsync(request, ct);
            return Ok(ApiResponse<RegisterResponse>.Ok(dto));
        }

        // POST api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request,CancellationToken ct)
        {
            var dto = await _authService.LoginAsync(request, ct);
            return Ok(ApiResponse<LoginResponse>.Ok(dto));
        }
    }
}