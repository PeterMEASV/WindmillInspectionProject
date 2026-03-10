
public class Command
{
    public string Action { get; set; }

    // Optional fields depending on the command
    public int? Value { get; set; }
    public string? Reason { get; set; }
    public double? Angle { get; set; }

    // Factory methods for convenience
    public static Command SetInterval(int interval)
    {
        if (interval < 1 || interval > 60)
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be between 1 and 60 seconds.");

        return new Command
        {
            Action = "setInterval",
            Value = interval
        };
    }

    public static Command Stop(string? reason = null)
    {
        return new Command
        {
            Action = "stop",
            Reason = reason
        };
    }

    public static Command Start()
    {
        return new Command
        {
            Action = "start"
        };
    }

    public static Command SetPitch(double angle)
    {
        if (angle < 0 || angle > 30)
            throw new ArgumentOutOfRangeException(nameof(angle), "Pitch angle must be between 0 and 30 degrees.");

        return new Command
        {
            Action = "setPitch",
            Angle = angle
        };
    }
}