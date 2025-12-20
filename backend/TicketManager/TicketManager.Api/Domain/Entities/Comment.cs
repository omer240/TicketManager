using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = default!;
        public string Text { get; set; } = default!;

        public string CreatedByUserId { get; set; } = default!;
        public ApplicationUser CreatedByUser { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
