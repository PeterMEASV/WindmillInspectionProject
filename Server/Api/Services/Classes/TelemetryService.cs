using DataAccess;
using Api.Services.Interfaces;

public class TelemetryService : ITelemetryService
{
    private readonly MyDbContext _context;

    public TelemetryService(MyDbContext context)
    {
        _context = context;
    }

    public async Task SaveTelemetryAsync(Telemetry telemetry)
    {
        _context.Telemetries.Add(telemetry);
        await _context.SaveChangesAsync();
        
    }
}