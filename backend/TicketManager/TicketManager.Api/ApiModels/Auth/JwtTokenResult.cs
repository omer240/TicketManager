namespace TicketManager.Api.ApiModels.Auth
{
    public class JwtTokenResult
    {
        public string AccessToken { get; init; } = default!;
        public DateTimeOffset ExpiresAt { get; init; }
    }
}
