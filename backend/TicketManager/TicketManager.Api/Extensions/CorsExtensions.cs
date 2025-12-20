using TicketManager.Api.Settings;

namespace TicketManager.Api.Extensions
{
    public static class CorsExtensions
    {
        private const string CorsPolicyName = "DefaultCors";

        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var cors = configuration.GetSection("Cors").Get<CorsSettings>()
                       ?? throw new InvalidOperationException("Cors settings missing.");

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(cors.AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();

                    if (cors.AllowCredentials)
                        builder.AllowCredentials();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(CorsPolicyName);
            return app;
        }
    }
}
