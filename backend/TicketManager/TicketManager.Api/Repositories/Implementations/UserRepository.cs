using Microsoft.EntityFrameworkCore;
using TicketManager.Api.ApiModels.Users;
using TicketManager.Api.Data.Contexts;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Repositories.Interfaces;

namespace TicketManager.Api.Repositories.Implementations
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAssigneesAsync(string currentUserId,string? search,
            CancellationToken ct = default)
        {
            var q = _context.Users
                .AsNoTracking()
                .Where(u => u.Id != currentUserId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(u => u.FullName != null && u.FullName.Contains(s));
            }

            return await q
                .OrderBy(u => u.FullName)
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    FullName = u.FullName!
                })
                .ToListAsync(ct);
        }
    }
}
