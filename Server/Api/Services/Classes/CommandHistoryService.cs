using DataAccess;

namespace Api.Services;

public class CommandHistoryService
{
    private readonly MyDbContext _context;

    public CommandHistoryService(MyDbContext context)
    {
        _context = context;
    }

    public async Task SaveCommandHistory(Command command, string turbineId, string? operatorId = null)
    {
        var history = new Commandhistory
        {
            Id = Guid.NewGuid().ToString(),
            Turbineid = turbineId,
            Action = command.Action,
            Value = command.Value,
            Angle = command.Angle,
            Reason = command.Reason,
            Timestamp = DateTime.UtcNow,
            Operatorid = operatorId
        };

        _context.Commandhistories.Add(history);
        await _context.SaveChangesAsync();
    }
}