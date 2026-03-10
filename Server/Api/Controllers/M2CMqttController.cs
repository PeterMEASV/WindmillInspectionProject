using System.Text.Json;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using Newtonsoft.Json.Linq;
using StateleSSE.AspNetCore;

namespace Api.Controllers;

public class M2CMqttController(ILogger<M2CMqttController> logger, MyDbContext context, IMqttClientService mqtt, ISseBackplane backplane) : MqttController
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
    
    [HttpPost("farm/Mindst2Commits/windmill/{turbineId}/command")]
    public async Task SetInterval(string turbineId, int interval)
    {
        var command = new Commands()
        {
            action = "setInterval",
            value = interval
        };
        await mqtt.PublishAsync($"farm/Mindst2Commits/windmill/{turbineId}/command", 
            JsonSerializer.Serialize(command));
    }
    
}