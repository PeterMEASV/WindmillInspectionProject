using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.EfRealtime;


namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController(ISseBackplane backplane, MyDbContext context, IRealtimeManager realtimeManager) 
    : RealtimeControllerBase(backplane)
{
    
    [HttpGet("{turbineId}")]
    public async Task<IActionResult> GetTelemetryForTurbine(string turbineId)
    {
        var data = await context.Telemetries
            .Where(t => t.Turbineid == turbineId)
            .OrderByDescending(t => t.Timestamp)
            .Take(100)
            .ToListAsync();

        return Ok(data);
    }
    
    
    [HttpPost(nameof(SwitchGroup))]
    public async Task SwitchGroup(string connectionId, string group, string? previousGroup)
    {
        if (previousGroup != null)
        {
            await backplane.Groups.RemoveFromGroupAsync(connectionId, previousGroup);
            Console.WriteLine($"Unsubscribe called for {connectionId}");
        }
        await backplane.Groups.AddToGroupAsync(connectionId, group);
        Console.WriteLine($"Subscribe called for {connectionId}");
    }
    
}