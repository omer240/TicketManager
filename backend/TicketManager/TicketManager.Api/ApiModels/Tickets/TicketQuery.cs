using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.ApiModels.Tickets
{
    public sealed record TicketQuery
    {
        public string? Search { get; init; }
        public TicketStatus? Status { get; init; }
        public TicketPriority? Priority { get; init; }
        public string? AssignedToUserId { get; init; }
        public string? CreatedByUserId { get; init; }

        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
