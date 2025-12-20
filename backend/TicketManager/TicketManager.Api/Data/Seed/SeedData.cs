using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketManager.Api.Data.Contexts;
using TicketManager.Api.Domain.Entities;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.Data.Seed;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var db = sp.GetRequiredService<AppDbContext>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync(ct);

        // Users (herkes eşit)
        var u1 = await EnsureUserAsync(userManager, "ayse@ticketmanager.local", "Ayşe Yılmaz", "User123!", ct);
        var u2 = await EnsureUserAsync(userManager, "mehmet@ticketmanager.local", "Mehmet Demir", "User123!", ct);
        var u3 = await EnsureUserAsync(userManager, "zeynep@ticketmanager.local", "Zeynep Kaya", "User123!", ct);
        var u4 = await EnsureUserAsync(userManager, "test1@ticketmanager.local", "Test User", "User123!", ct);

        // Daha önce seed basıldıysa tekrar basmayalım
        if (await db.Tickets.AnyAsync(ct))
            return;

        var tickets = new List<Ticket>
        {
            new()
            {
                Title = "Login sonrası 401 dönüyor",
                Description = "Kullanıcı giriş yaptıktan sonra bazı endpointlerde 401 alıyoruz. Token süresi / audience kontrol edilmeli.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.High,
                CreatedByUserId = u1.Id,
                AssignedToUserId = u2.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-3),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            },
            new()
            {
                Title = "Ticket filtreleme endpointi eklenmeli",
                Description = "Status + Priority + date aralığı ile filtreleme yapılacak endpoint isteniyor.",
                Status = TicketStatus.InProgress,
                Priority = TicketPriority.Medium,
                CreatedByUserId = u2.Id,
                AssignedToUserId = u3.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-2),
            },
            new()
            {
                Title = "UI tarafında CORS hatası",
                Description = "Angular 4200’den istek atınca CORS error alınıyor. AllowedOrigins ayarları kontrol edilmeli.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.Low,
                CreatedByUserId = u3.Id,
                AssignedToUserId = u4.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-20),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-5),
            }
        };

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync(ct);

        var comments = new List<Comment>
        {
            new()
            {
                TicketId = tickets[0].Id,
                Text = "Token üretiminde Issuer/Audience kontrolünü bir daha gözden geçirelim.",
                CreatedByUserId = u2.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-2),
            },
            new()
            {
                TicketId = tickets[0].Id,
                Text = "Refresh token yoksa kısa süreli tokenlar kullanıcıyı düşürebilir.",
                CreatedByUserId = u1.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-2).AddHours(2),
            },
            new()
            {
                TicketId = tickets[1].Id,
                Text = "Filtre parametreleri için TicketQuery gibi bir model iyi olur.",
                CreatedByUserId = u3.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-4),
            },
            new()
            {
                TicketId = tickets[2].Id,
                Text = "CORS AllowedOrigins doğru ama AllowCredentials false iken frontend tarafı credentials yolluyorsa patlar.",
                CreatedByUserId = u4.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-6),
            }
        };

        db.Comments.AddRange(comments);
        await db.SaveChangesAsync(ct);
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string password,
        CancellationToken ct)
    {
        var normalizedEmail = email.ToUpperInvariant();

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, ct);

        if (user is not null)
            return user;

        user = new ApplicationUser
        {
            Email = email,
            // Username istemiyorsun: biz otomatik email veriyoruz.
            UserName = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var msg = string.Join(" | ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            throw new InvalidOperationException($"Seed user create failed: {email} => {msg}");
        }

        return user;
    }
}
