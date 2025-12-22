namespace TicketManager.Api.ApiModels.Auth.Login
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = default!;
        public DateTimeOffset ExpiresAt { get; set; }
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
}
