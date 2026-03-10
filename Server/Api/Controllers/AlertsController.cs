using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.EfRealtime;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlertsController(ISseBackplane backplane, MyDbContext context, IRealtimeManager realtimeManager) 
    : RealtimeControllerBase(backplane) 
{
    [HttpPost]
    public async Task<IActionResult> CreateAlert([FromBody] Alert data)
    {
        data.Id = Guid.NewGuid().ToString();

        context.Alerts.Add(data);
        await context.SaveChangesAsync();

        await backplane.Clients.SendToGroupAsync(data.Turbineid, data);

        return Ok(new
        {
            message = "Alert created successfully",
            alertId = data.Id
        });

    }
    
    [HttpGet("turbine/{turbineId}")]
    public async Task<IActionResult> GetAlertsForTurbine(string turbineId)
    {
        var alerts = await context.Alerts
            .Where(a => a.Turbineid == turbineId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        return Ok(alerts);
    }

    [HttpPost(nameof(SwitchAlertGroup))]
    public async Task SwitchAlertGroup(string connectionId, string turbineId, string? previousTurbineId)
    {
        if (previousTurbineId != null)
        {
            await backplane.Groups.RemoveFromGroupAsync(connectionId, previousTurbineId);
            Console.WriteLine($"Unsubscribed {connectionId} from {previousTurbineId}");
        }

        await backplane.Groups.AddToGroupAsync(connectionId, turbineId);
        Console.WriteLine($"Subscribed {connectionId} to {turbineId}");
    }
    

}