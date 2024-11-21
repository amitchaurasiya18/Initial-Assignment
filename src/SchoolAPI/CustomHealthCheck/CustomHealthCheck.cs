using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolAPI.CustomHealthCheck
{
    public class CustomHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public CustomHealthCheck(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _httpClient.GetAsync("http://localhost:5206/Student");

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
