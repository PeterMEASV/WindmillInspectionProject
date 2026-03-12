namespace Api.Services.Interfaces;
using DataAccess;

public interface ITelemetryService
{
    Task SaveTelemetryAsync(Telemetry telemetry);
}