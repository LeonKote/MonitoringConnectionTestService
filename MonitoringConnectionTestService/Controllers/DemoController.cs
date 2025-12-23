using Microsoft.AspNetCore.Mvc;
using MonitoringConnectionLib.Interfaces;

namespace MonitoringConnectionTestService.Controllers;

[ApiController]
[Route("/api/demo")]
public class DemoController : ControllerBase
{
    private readonly IMetricsRegistry _metrics;
    private readonly ICounter _myRequests;
    private readonly ILogger<DemoController> _logger;

    public DemoController(IMetricsRegistry metrics, ILogger<DemoController> logger)
    {
        _metrics = metrics;
        // создаём счётчик с label'ом "method"
        _myRequests = _metrics.GetOrCreateCounter("demo_requests_total", "Total requests to demo endpoint", "method");
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _myRequests.Inc(1, "Get");
        _logger.LogInformation("Test endpoint called. CorrelationId: {CorrelationId}", Guid.NewGuid());
        return Ok(new { now = DateTime.UtcNow });
    }

    [HttpPost]
    public IActionResult Post()
    {
        _myRequests.Inc(1, "Post");
        return Ok();
    }
}
