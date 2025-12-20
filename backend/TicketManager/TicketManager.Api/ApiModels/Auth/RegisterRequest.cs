using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.ApiModels.Auth
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(3), MaxLength(150)]
        public string FullName { get; set; } = default!;

        [Required, MinLength(6)]
        public string Password { get; set; } = default!;
    }
}
