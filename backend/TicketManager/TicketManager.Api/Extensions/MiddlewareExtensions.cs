using TicketManager.Api.ApiModels.Common.Middleware;

namespace TicketManager.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiPipeline(this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiExceptionMiddleware>();
            return app;
        }
    }
}
