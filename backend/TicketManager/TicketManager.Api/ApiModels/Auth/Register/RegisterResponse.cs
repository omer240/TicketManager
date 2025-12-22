namespace TicketManager.Api.ApiModels.Auth.Register
{
    public class RegisterResponse
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
}
