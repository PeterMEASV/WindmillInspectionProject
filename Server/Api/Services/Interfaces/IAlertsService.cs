using DataAccess;

namespace Api.Services.Interfaces;

public interface IAlertsService
{
    Task<List<Alert>> GetAlertsForTurbine(string turbineId);
    Task<List<Alert>> GetAllAlerts();
    Task<bool> ResolveAlert(string id);
    Task<Alert> SaveAlertAsync(Alert alert);
}