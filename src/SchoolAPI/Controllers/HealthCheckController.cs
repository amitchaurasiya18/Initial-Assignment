using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            HealthReport report = await _healthCheckService.CheckHealthAsync();
            return Ok(report);
        }
    }
}