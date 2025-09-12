using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bloggy.Core.Utilities.HealthChecks;

public class GeneralHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        return await Task.FromResult(HealthCheckResult.Healthy("Everything is Ok!"));
    }
}
