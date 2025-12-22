using TicketManager.Api.ApiModels.Auth.Login;
using TicketManager.Api.ApiModels.Auth.Register;

namespace TicketManager.Api.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
