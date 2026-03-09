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
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestTelemetry()
    {
        var data = await context.Telemetries
            .GroupBy(t => t.Turbineid)
            .Select(g => g.OrderByDescending(t => t.Timestamp).First())
            .ToListAsync();

        return Ok(data);
    }
    
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

    [HttpGet(nameof(GetTelemetry))]
    public async Task<RealtimeListenResponse<List<Telemetry>>> GetTelemetry(string connectionId)
    {
        var group = "telemetry";
        await backplane.Groups.AddToGroupAsync(connectionId, group);
        Console.WriteLine($"Subscribe called for {connectionId}");
        realtimeManager.Subscribe<MyDbContext>(
            connectionId,
            group,
            criteria: snapshot => snapshot.HasAdded<Telemetry>(),
            query: async context => await context.Telemetries
                .OrderByDescending(t => t.Timestamp)
                .Take(1)
                .ToListAsync()
        );
        
        return new RealtimeListenResponse<List<Telemetry>>(group, context.Telemetries.ToList());
    }
    
}