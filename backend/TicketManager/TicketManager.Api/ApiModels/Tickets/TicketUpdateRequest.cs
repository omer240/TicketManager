using System.ComponentModel.DataAnnotations;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.ApiModels.Tickets
{
    public class TicketUpdateRequest
    {
        [Required(ErrorMessage = "Başlık zorunludur.")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [MaxLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        public string Description { get; set; } = default!;

        [EnumDataType(typeof(TicketPriority), ErrorMessage = "Geçersiz öncelik değeri.")]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    }
}
