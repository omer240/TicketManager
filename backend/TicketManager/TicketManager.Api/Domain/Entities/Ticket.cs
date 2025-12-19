using System.ComponentModel.DataAnnotations;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        public string CreatedByUserId { get; set; } = default!;
        public ApplicationUser CreatedByUser { get; set; } = default!;

        public string? AssignedToUserId { get; set; }
        public ApplicationUser? AssignedToUser { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
