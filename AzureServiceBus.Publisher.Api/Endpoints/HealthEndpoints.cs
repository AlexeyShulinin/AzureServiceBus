using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace AzureServiceBus.Publisher.Api.Endpoints;

public static class HealthEndpoints
{
    public static void RegisterHealthEndpoints(this WebApplication app)
    {
        app.MapGet("health", () => Task.FromResult("healthy"));
    }
}