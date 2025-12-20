using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Threading.RateLimiting;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Responses;
using TicketManager.Api.Settings;

namespace TicketManager.Api.Extensions
{
    public static class RateLimitExtensions
    {
        public const string AuthPolicy = "AuthPolicy";
        public const string DefaultPolicy = "DefaultPolicy";

        public static IServiceCollection AddRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection("RateLimit").Get<RateLimitSettings>()
                           ?? throw new InvalidOperationException("RateLimit settings missing.");

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, ct) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = ApiResponse<object>.Fail(
                        ErrorCodes.RateLimited,
                        "Too many requests. Please try again later.");

                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(response),
                        ct);
                };

                options.AddFixedWindowLimiter(AuthPolicy, limiter =>
                {
                    limiter.PermitLimit = settings.AuthPermitLimit;
                    limiter.Window = TimeSpan.FromSeconds(settings.AuthWindowSeconds);
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiter.QueueLimit = settings.AuthQueueLimit;
                });

                options.AddFixedWindowLimiter(DefaultPolicy, limiter =>
                {
                    limiter.PermitLimit = settings.PermitLimit;
                    limiter.Window = TimeSpan.FromSeconds(settings.WindowSeconds);
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiter.QueueLimit = settings.QueueLimit;
                });
            });

            return services;
        }

        public static IApplicationBuilder UseRateLimitingPolicy(this IApplicationBuilder app)
        {
            app.UseRateLimiter();
            return app;
        }
    }
}
