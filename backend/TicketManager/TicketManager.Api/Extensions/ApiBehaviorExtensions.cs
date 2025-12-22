using Microsoft.AspNetCore.Mvc;
using TicketManager.Api.ApiModels.Common.Exceptions;
using TicketManager.Api.ApiModels.Common.Responses;

namespace TicketManager.Api.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection AddApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstErrorMessage = context.ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault()
                        ?? "Gönderilen veriler geçerli değil.";

                    var payload = ApiResponse<EmptyDto>.Fail(
                        ErrorCodes.Validation,
                        firstErrorMessage
                    );

                    return new BadRequestObjectResult(payload);
                };
            });

            return services;
        }
    }
}
