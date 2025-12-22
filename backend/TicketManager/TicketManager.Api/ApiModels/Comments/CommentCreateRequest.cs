using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.ApiModels.Comments
{
    public class CommentCreateRequest
    {
        [Required(ErrorMessage = "Yorum metni zorunludur.")]
        [MaxLength(2000, ErrorMessage = "Yorum metni en fazla 2000 karakter olabilir.")]
        public string Text { get; set; } = default!;
    }
}
