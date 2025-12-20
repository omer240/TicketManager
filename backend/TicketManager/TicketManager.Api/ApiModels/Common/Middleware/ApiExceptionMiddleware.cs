using System.Text.Json;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Responses;

namespace TicketManager.Api.ApiModels.Common.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;

                var response = ApiResponse<object>.Fail(ex.Code, ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = ApiResponse<object>.Fail("server_error", "Unexpected error occurred.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
