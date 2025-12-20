namespace TicketManager.Api.ApiModels.Common.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string Code { get; }

        public ApiException(int statusCode, string code, string message)
            : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }

        public static ApiException NotFound(string message = "Not found")
            => new(404, ErrorCodes.NotFound, message);

        public static ApiException Forbidden(string message = "Forbidden")
            => new(403, ErrorCodes.Forbidden, message);

        public static ApiException BadRequest(string message = "Bad request")
            => new(400, ErrorCodes.BadRequest, message);

        public static ApiException Validation(string message = "Validation error")
            => new(400, ErrorCodes.Validation, message);

        public static ApiException Unauthorized(string message = "Unauthorized")
            => new(401, ErrorCodes.Unauthorized, message);

        public static ApiException ServerError(string message = "Server error")
            => new(500, ErrorCodes.ServerError, message);
    }
}
