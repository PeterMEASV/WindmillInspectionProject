namespace Api.Models;

public class AlertDTO
{
    public string Turbineid { get; set; } = null!;

    public string Farmid { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public string Severity { get; set; } = null!;

    public string Message { get; set; } = null!;
}