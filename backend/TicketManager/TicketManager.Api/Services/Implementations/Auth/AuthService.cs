using Microsoft.AspNetCore.Identity;
using TicketManager.Api.ApiModels.Auth;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Services.Interfaces.Auth;

namespace TicketManager.Api.Services.Implementations.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwt;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw ApiException.BadRequest("Email zorunludur.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw ApiException.BadRequest("FullName zorunludur.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw ApiException.BadRequest("Password zorunludur.");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing is not null)
                throw ApiException.BadRequest("Bu email zaten kayıtlı.");

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName.Trim()
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw ApiException.BadRequest(string.Join(" | ", result.Errors.Select(e => e.Description)));

            var jwtTokenResult = _jwt.CreateToken(user);

            return new AuthResponse
            {
                AccessToken = jwtTokenResult.AccessToken,
                ExpiresAt = jwtTokenResult.ExpiresAt,
                UserId = user.Id,
                Email = user.Email ?? request.Email,
                FullName = user.FullName ?? ""
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw ApiException.BadRequest("Email zorunludur.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw ApiException.BadRequest("Password zorunludur.");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw ApiException.BadRequest("Email veya şifre hatalı.");

            var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!check.Succeeded)
                throw ApiException.BadRequest("Email veya şifre hatalı.");

            var jwtTokenResult = _jwt.CreateToken(user);

            return new AuthResponse
            {
                AccessToken = jwtTokenResult.AccessToken,
                ExpiresAt = jwtTokenResult.ExpiresAt,
                UserId = user.Id,
                Email = user.Email ?? request.Email,
                FullName = user.FullName ?? ""
            };
        }
    }
}
