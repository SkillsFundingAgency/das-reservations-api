using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;

namespace SFA.DAS.Reservations.Api.StartupConfig
{
    public static class HealthCheckStartup
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });
            
            app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                Predicate = (_) => false
            });

            return app;
        }
    }
}
