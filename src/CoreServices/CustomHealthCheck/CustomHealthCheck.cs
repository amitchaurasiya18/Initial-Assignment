using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CoreServices.CustomHealthCheck
{
    public class CustomHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly string? _healthCheckUrl;

        public CustomHealthCheck(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _healthCheckUrl = configuration["HealthCheckUrl"];
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_healthCheckUrl);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(new HealthCheckResult(
                    status: HealthStatus.Healthy,
                    description: "The api is up and running"
                ));
            }

            return await Task.FromResult(new HealthCheckResult(
                    status: HealthStatus.Unhealthy,
                    description: "The api is down"
            ));
        }
    }
}
