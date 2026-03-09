using System.Text.Json;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using Newtonsoft.Json.Linq;

namespace Api.Controllers;

public class M2CMqttController(ILogger<M2CMqttController> logger, MyDbContext context, IMqttClientService mqtt) : MqttController
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