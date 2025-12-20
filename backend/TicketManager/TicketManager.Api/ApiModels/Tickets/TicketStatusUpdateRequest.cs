using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.ApiModels.Tickets
{
    public class TicketStatusUpdateRequest
    {
        public int TicketId { get; set; }
        public TicketStatus Status { get; set; }
    }
}
