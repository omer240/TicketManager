using System.ComponentModel.DataAnnotations;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.ApiModels.Tickets
{
    public class TicketUpdateRequest
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = default!;

        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }

        [Required]
        public string AssignedToUserId { get; set; } = default!;
    }
}
