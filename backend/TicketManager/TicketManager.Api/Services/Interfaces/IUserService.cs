using TicketManager.Api.ApiModels.Users;

namespace TicketManager.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> GetAssigneesAsync(string currentUserId, string? search = null, CancellationToken ct = default);
    }
}
