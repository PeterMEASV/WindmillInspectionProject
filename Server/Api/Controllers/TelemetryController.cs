using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<List<Telemetry>> GetTelemetryForTurbine(string turbineId)
    {
        var data = await context.Telemetries
            .Where(t => t.Turbineid == turbineId)
            .OrderByDescending(t => t.Timestamp)
            .Take(100)
            .ToListAsync();

        return data;
    }

    [HttpPost(nameof(AddToGroup))]
    public async Task<IActionResult> AddToGroup(string connectionId, string group)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(group))
        {
            return BadRequest("connectionId and group are required.");
        }

        await backplane.Groups.AddToGroupAsync(connectionId, group);
        Console.WriteLine($"Subscribed {connectionId} to {group}");

        return Ok(new
        {
            message = $"Subscribed to group '{group}'"
        });
    }

    [HttpPost(nameof(RemoveFromGroup))]
    public async Task<IActionResult> RemoveFromGroup(string connectionId, string group)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(group))
        {
            return BadRequest("connectionId and group are required.");
        }

        await backplane.Groups.RemoveFromGroupAsync(connectionId, group);
        Console.WriteLine($"Unsubscribed {connectionId} from {group}");

        return Ok(new
        {
            message = $"Unsubscribed from group '{group}'"
        });
    }

    [HttpPost(nameof(SwitchGroup))]
    public async Task<IActionResult> SwitchGroup(string connectionId, string group, string? previousGroup)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(group))
        {
            return BadRequest("connectionId and group are required.");
        }

        if (!string.IsNullOrWhiteSpace(previousGroup))
        {
            await backplane.Groups.RemoveFromGroupAsync(connectionId, previousGroup);
            Console.WriteLine($"Unsubscribed {connectionId} from {previousGroup}");
        }
        await backplane.Groups.AddToGroupAsync(connectionId, group);
        Console.WriteLine($"Subscribed {connectionId} to {group}");

        return Ok(new
        {
            message = $"Switched subscription to '{group}'",
            previousGroup
        });
    }

    [HttpPost(nameof(SubscribeToAllAlerts))]
    
    public async Task<IActionResult> SubscribeToAllAlerts(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            return BadRequest("connectionId is required.");
        }

        await backplane.Groups.AddToGroupAsync(connectionId, "alerts-all");
        Console.WriteLine($"Subscribed {connectionId} to alerts-all");

        return Ok(new
        {
            message = "Subscribed to all alerts.",
            group = "alerts-all"
        });
    }
}