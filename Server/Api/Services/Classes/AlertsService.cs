using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class AlertsService(MyDbContext context) : IAlertsService
{
    public Task<List<Alert>> GetAlertsForTurbine(string turbineId)
    {
        return  context.Alerts
            .Where(a => a.Turbineid == turbineId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public Task<List<Alert>> GetAllAlerts()
    {
        return context.Alerts
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<bool> ResolveAlert(string id)
    {
        var alert = await context.Alerts.FindAsync(id);
        if (alert == null)
        {
            return false;
        }

        alert.Resolved = true;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Alert> SaveAlertAsync(Alert alert)
    {
        alert.Id = Guid.NewGuid().ToString();
        alert.Timestamp ??= DateTime.UtcNow;
        alert.Resolved = false;

        context.Alerts.Add(alert);
        await context.SaveChangesAsync();
        return alert;
    }
}