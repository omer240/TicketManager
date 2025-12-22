using System.ComponentModel.DataAnnotations;

namespace TicketManager.Api.ApiModels.Auth.Register
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [MinLength(3, ErrorMessage = "Ad soyad en az 3 karakter olmalıdır.")]
        [MaxLength(150, ErrorMessage = "Ad soyad en fazla 150 karakter olabilir.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = default!;
    }
}
