using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Alert
{
    public string Id { get; set; } = null!;

    public string Turbineid { get; set; } = null!;

    public string Farmid { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public string Severity { get; set; } = null!;

    public string Message { get; set; } = null!;
}
