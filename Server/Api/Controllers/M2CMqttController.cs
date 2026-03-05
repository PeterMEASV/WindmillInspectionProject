using System.Text.Json;
using Mqtt.Controllers;

namespace Api.Controllers;

public class M2CMqttController(ILogger<M2CMqttController> logger) : MqttController
{
    [MqttRoute("farm/Mindst2Commits/windmill/{turbineId}/telemetry")]
    public async Task SubscribeToWindmillTelemetry(object data, string turbineId)
    {
        logger.LogInformation("Data from the turbine: " + turbineId);
        logger.LogInformation(JsonSerializer.Serialize(data));
        
    }
    
    
}