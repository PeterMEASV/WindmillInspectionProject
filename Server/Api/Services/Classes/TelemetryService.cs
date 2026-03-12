using DataAccess;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class TelemetryService(MyDbContext context) : ITelemetryService
{
    public async Task SaveTelemetryAsync(Telemetry telemetry)
    {
        context.Telemetries.Add(telemetry);
        await context.SaveChangesAsync();
    }

    public Task<List<Telemetry>> GetTelemetryForTurbine(string turbineId)
    {
        return context.Telemetries
            .Where(t => t.Turbineid == turbineId)
            .OrderByDescending(t => t.Timestamp)
            .Take(100)
            .ToListAsync();
    }
}