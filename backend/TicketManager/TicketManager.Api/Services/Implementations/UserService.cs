using TicketManager.Api.ApiModels.Users;
using TicketManager.Api.Repositories.Interfaces;
using TicketManager.Api.Services.Interfaces;

namespace TicketManager.Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IReadOnlyList<UserDto>> GetAssigneesAsync(
            string currentUserId,
            string? search,
            CancellationToken ct = default)
        {
            var users = await _userRepo.GetAssigneesAsync(currentUserId, search, ct);

            return users
                .Where(u => u is not null) 
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName ?? string.Empty
                })
                .ToList();
        }
    }
}
