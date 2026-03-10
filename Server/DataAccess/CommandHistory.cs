namespace DataAccess;

public class CommandHistory
{
    public int Id { get; set; }               // primary key
    public string TurbineId { get; set; }     // which turbine
    public string Action { get; set; }        // start, stop, setPitch, etc.
    public int? Value { get; set; }           // optional numeric value (interval)
    public double? Angle { get; set; }        // optional pitch angle
    public string? Reason { get; set; }       // optional reason
    public DateTime Timestamp { get; set; }   // when command was sent
    public string? OperatorId { get; set; }   // optional, if you have auth
}