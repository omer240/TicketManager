using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.ApiModels.Comments
{
    public class CommentUpdateRequest
    {
        [Required, MaxLength(2000)]
        public string Text { get; set; } = default!;
    }
}
