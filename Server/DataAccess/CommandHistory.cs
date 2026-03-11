using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Commandhistory
{
    public string Id { get; set; } = null!;

    public string Turbineid { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string Action { get; set; } = null!;

    public int? Value { get; set; }

    public double? Angle { get; set; }

    public string? Reason { get; set; }

    public string? Operatorid { get; set; }
}
