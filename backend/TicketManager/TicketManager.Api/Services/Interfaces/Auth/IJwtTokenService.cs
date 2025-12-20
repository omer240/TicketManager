using TicketManager.Api.ApiModels.Auth;
using TicketManager.Api.Domain.Entities;

namespace TicketManager.Api.Services.Interfaces.Auth
{
    public interface IJwtTokenService
    {
        JwtTokenResult CreateToken(ApplicationUser user);
    }
}
