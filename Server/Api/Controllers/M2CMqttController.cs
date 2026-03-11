using System.Text.Json;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Mqtt.Controllers;
using StateleSSE.AspNetCore;

namespace Api.Controllers;

public class M2CMqttController(ILogger<M2CMqttController> logger, MyDbContext context, IMqttClientService mqtt, ISseBackplane backplane, CommandHistoryService commandHistoryService) : MqttController
{
    
    [MqttRoute("farm/Mindst2Commits/windmill/{turbineId}/telemetry")]
    public async Task SubscribeToWindmillTelemetry(Telemetry data, string turbineId)
    {
        logger.LogInformation($"Data from turbine: {turbineId}");   

        logger.LogInformation($"Wind speed: {data.Windspeed}");
        logger.LogInformation($"Temperature: {data.Ambienttemperature}");
        logger.LogInformation($"Power output: {data.Poweroutput}");
        logger.LogInformation($"Status: {data.Status}");
        
        data.Id = Guid.NewGuid().ToString();
        
        context.Telemetries.Add(data);
        await context.SaveChangesAsync();

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
    public async Task<IActionResult> PublishAlert(string turbineId, [FromBody] Alert alert)
    {
        alert.Turbineid = turbineId;

        await mqtt.PublishAsync(
            $"farm/Mindst2Commits/windmill/{turbineId}/alert",
            JsonSerializer.Serialize(alert));

        return new OkObjectResult(new
        {
            message = "Alert published to MQTT",
            turbineId
        });
    }
    
    [MqttRoute("farm/Mindst2Commits/windmill/{turbineId}/alert")]
    public async Task SubscribeToWindmillAlerts(Alert data, string turbineId)
    {
        logger.LogInformation($"Data from turbine: {turbineId}");   

        logger.LogInformation($"Timestamp: {data.Timestamp}");
        logger.LogInformation($"Severity: {data.Severity}");
        logger.LogInformation($"Message: {data.Message}");
    
        data.Id = Guid.NewGuid().ToString();
    
        context.Alerts.Add(data);
        await context.SaveChangesAsync();

        await backplane.Clients.SendToGroupAsync(data.Turbineid,data);
    }
    
    
    
}