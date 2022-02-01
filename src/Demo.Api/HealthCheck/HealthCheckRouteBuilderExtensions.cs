using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Demo.Api.HealthCheck
{
    public static class HealthCheckRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponses.WriteJsonResponse
            });

            return endpoints;
        }
    }
}