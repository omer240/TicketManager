using System.ComponentModel.DataAnnotations;
using TicketManager.Api.Domain.Enums;

namespace TicketManager.Api.ApiModels.Tickets
{
    public class TicketStatusUpdateRequest
    {
        [Required(ErrorMessage = "Durum zorunludur.")]
        [EnumDataType(typeof(TicketStatus), ErrorMessage = "Geçersiz durum değeri.")]
        public TicketStatus Status { get; set; }
    }
}
