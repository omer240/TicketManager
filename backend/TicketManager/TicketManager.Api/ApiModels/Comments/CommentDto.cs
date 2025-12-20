namespace TicketManager.Api.ApiModels.Comments
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Text { get; set; } = default!;
        public string CreatedByUserId { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
