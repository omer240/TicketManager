using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.ApiModels.Comments
{
    public class CommentCreateRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required, MaxLength(2000)]
        public string Text { get; set; } = default!;
    }
}
