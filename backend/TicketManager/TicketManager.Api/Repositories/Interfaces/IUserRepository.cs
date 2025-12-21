using TicketManager.Api.ApiModels.Users;
using TicketManager.Api.Domain.Entities;

namespace TicketManager.Api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<ApplicationUser>> GetAssigneesAsync(string currentUserId,string? search = null,CancellationToken ct = default);
    }
}
