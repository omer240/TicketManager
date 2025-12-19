using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Text { get; set; } = default!;

        [Required]
        public string CreatedByUserId { get; set; } = default!;
        public ApplicationUser CreatedByUser { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
