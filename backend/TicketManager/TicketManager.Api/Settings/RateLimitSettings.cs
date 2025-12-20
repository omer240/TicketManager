namespace TicketManager.Api.Settings
{
    public class RateLimitSettings
    {
        public int PermitLimit { get; set; } = 100;
        public int WindowSeconds { get; set; } = 60;
        public int QueueLimit { get; set; } = 0;

        public int AuthPermitLimit { get; set; } = 10;
        public int AuthWindowSeconds { get; set; } = 60;
        public int AuthQueueLimit { get; set; } = 0;
    }
}
