using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlertsController(MyDbContext context) : ControllerBase
{
    [HttpGet("turbine/{turbineId}")]
    public async Task<IActionResult> GetAlertsForTurbine(string turbineId)
    {
        var alerts = await context.Alerts
            .Where(a => a.Turbineid == turbineId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        return Ok(alerts);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAlerts()
    {
        var alerts = await context.Alerts
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        return Ok(alerts);
    }
}