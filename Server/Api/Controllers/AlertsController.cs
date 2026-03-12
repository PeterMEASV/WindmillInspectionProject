using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlertsController(IAlertsService alertsService) : ControllerBase
{
    [HttpGet("turbine/{turbineId}")]
    public async Task<IActionResult> GetAlertsForTurbine(string turbineId)
    {
        var alerts = await alertsService.GetAlertsForTurbine(turbineId);
        return Ok(alerts);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAlerts()
    {
        var alerts = await alertsService.GetAllAlerts();
        return Ok(alerts);
    }

    [HttpPatch]
    public async Task<IActionResult> ResolveAlert(string id)
    {
        var resolved = await alertsService.ResolveAlert(id);
        if (!resolved)
        {
            return NotFound();
        }

        return NoContent();
    }
}