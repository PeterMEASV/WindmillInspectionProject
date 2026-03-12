using System.Text.Json;
using Api.Models;
using Api.Services;
using DataAccess;
using Api.Services.Interfaces;
//using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Mqtt.Controllers;
using StateleSSE.AspNetCore;

namespace Api.Controllers;

public class M2CMqttController(ILogger<M2CMqttController> logger, MyDbContext context,ITelemetryService telemetryService, IMqttClientService mqtt, ISseBackplane backplane, CommandHistoryService commandHistoryService) : MqttController
{
    private readonly ILogger<M2CMqttController> _logger = logger;
    private readonly ITelemetryService _telemetryService = telemetryService;
    private readonly ISseBackplane _backplane = backplane;
    
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [MqttRoute("farm/Mindst2Commits/windmill/{turbineId}/telemetry")]
    public async Task SubscribeToWindmillTelemetry(Telemetry data, string turbineId)
    {
        logger.LogInformation($"Data from turbine: {turbineId}");

        logger.LogInformation($"Wind speed: {data.Windspeed}");
        logger.LogInformation($"Temperature: {data.Ambienttemperature}");
        logger.LogInformation($"Power output: {data.Poweroutput}");
        logger.LogInformation($"Status: {data.Status}");
        
        data.Id = Guid.NewGuid().ToString();
        data.Turbineid = turbineId;

        await _telemetryService.SaveTelemetryAsync(data);

        switch (data.Turbineid)
        {
            case "turbine-alpha":
                await backplane.Clients.SendToGroupAsync("turbine-alpha", data);
                break;
            case "turbine-beta":
                await backplane.Clients.SendToGroupAsync("turbine-beta", data);
                break;
            case "turbine-gamma":
                await backplane.Clients.SendToGroupAsync("turbine-gamma", data);
                break;
            case "turbine-delta":
                await backplane.Clients.SendToGroupAsync("turbine-delta", data);
                break;
            default:
                logger.LogError("Turbine ID not found");
                break;
        }
        logger.LogInformation("Telemetry saved to database");
        
    }
    
    [HttpPost("farm/Mindst2Commits/windmill/{turbineId}/command/set-interval")]
    public async Task<ActionResult> SetInterval(string turbineId, int interval)
    {
        var command = Command.SetInterval(interval);
        await mqtt.PublishAsync($"farm/Mindst2Commits/windmill/{turbineId}/command",
            JsonSerializer.Serialize(command));

        await commandHistoryService.SaveCommandHistory(command, turbineId);
        return new OkResult();
    }
    [HttpPost("farm/Mindst2Commits/windmill/{turbineId}/command/stop")]
    public async Task<ActionResult> StopTurbine(string turbineId, [FromQuery] string? reason = null)
    {
        var command = Command.Stop(reason);
        await mqtt.PublishAsync($"farm/Mindst2Commits/windmill/{turbineId}/command",
            JsonSerializer.Serialize(command));

        await commandHistoryService.SaveCommandHistory(command, turbineId);
        
        return new OkResult();
    }
    [HttpPost("farm/Mindst2Commits/windmill/{turbineId}/command/start")]
    public async Task<ActionResult> StartTurbine(string turbineId)
    {
        var command = Command.Start();
        await mqtt.PublishAsync($"farm/Mindst2Commits/windmill/{turbineId}/command",
            JsonSerializer.Serialize(command));
        
        await commandHistoryService.SaveCommandHistory(command, turbineId);
        
        return new OkResult();
    }
    [HttpPost("farm/Mindst2Commits/windmill/{turbineId}/command/blade-pitch")]
    public async Task<ActionResult> SetBladePitch(string turbineId, int bladePitch)
    {
        var command = Command.SetPitch(bladePitch);
        await mqtt.PublishAsync($"farm/Mindst2Commits/windmill/{turbineId}/command",
            JsonSerializer.Serialize(command));

        await commandHistoryService.SaveCommandHistory(command, turbineId);
            
        return new OkResult();
       
    }
    
    
    
    [HttpPost("farm/Mindst2Commits/windmill/{turbineId}/alert")]
    public async Task<IActionResult> PublishAlert([FromBody] Alert alert)
    {
        alert.Turbineid = alert.Turbineid;
        alert.Timestamp ??= DateTime.UtcNow;
        alert.Id = Guid.NewGuid().ToString();
        alert.Resolved = false;

        // Save to DB
        context.Alerts.Add(alert);
        await context.SaveChangesAsync();
        
        // 2) global alerts group
        await backplane.Clients.SendToGroupAsync("alerts-all", alert);

        logger.LogInformation("Simulated alert sent to SSE for turbine {TurbineId}", alert.Turbineid);

        return new OkObjectResult(new { message = "Alert published", alert });
    }

    [MqttRoute("farm/Mindst2Commits/windmill/{turbineId}/alert")]
        public async Task SubscribeToAlerts(AlertDTO alert, string turbineId)
        {
            logger.LogInformation("Alert received from Mindst2Commits for turbine {TurbineId}", turbineId);
            await backplane.Clients.SendToGroupAsync("alerts-all", alert);
            
            Alert data = new Alert
            {
                Id = Guid.NewGuid().ToString(),
                Turbineid = turbineId,
                Timestamp = alert.Timestamp,
                Message = alert.Message,
                Severity = alert.Severity
            };
            context.Alerts.Add(data);
            await context.SaveChangesAsync();
        }
    
}