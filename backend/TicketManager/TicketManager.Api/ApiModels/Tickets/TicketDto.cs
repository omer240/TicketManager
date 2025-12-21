using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.ApiModels.Tickets
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }

        public string CreatedByUserId { get; set; } = default!;
        public string? AssignedToUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }
        public string? AssignedToUserFullName { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public int CommentCount { get; set; }
    }
}
