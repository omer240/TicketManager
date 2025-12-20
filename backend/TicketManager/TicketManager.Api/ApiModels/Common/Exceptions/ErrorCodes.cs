namespace TicketManager.Api.ApiModels.Common.Exceptions
{
    public static class ErrorCodes
    {
        public const string NotFound = "not_found";
        public const string Forbidden = "forbidden";
        public const string BadRequest = "bad_request";
        public const string Validation = "validation_error";
        public const string Unauthorized = "unauthorized";
        public const string ServerError = "server_error";
        public const string RateLimited = "rate_limited";
    }
}
