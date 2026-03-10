using DataAccess;

namespace Api.Services;

public class CommandHistoryService
{
    public async Task SaveCommandHistory(MyDbContext context,Command command, string turbineId, string? operatorId = null)
    {
        var history = new CommandHistory
        {
            TurbineId = turbineId,
            Action = command.Action,
            Value = command.Value,
            Angle = command.Angle,
            Reason = command.Reason,
            Timestamp = DateTime.UtcNow,
            //OperatorId = operatorId
        };

        context.CommandHistories.Add(history);
        await context.SaveChangesAsync();
    }
}