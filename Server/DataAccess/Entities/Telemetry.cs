using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Telemetry
{
    public string Id { get; set; } = null!;

    public string Turbineid { get; set; } = null!;

    public string Turbinename { get; set; } = null!;

    public string Farmid { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public decimal? Windspeed { get; set; }

    public decimal? Winddirection { get; set; }

    public decimal? Ambienttemperature { get; set; }

    public decimal? Rotorspeed { get; set; }

    public decimal? Poweroutput { get; set; }

    public decimal? Nacelledirection { get; set; }

    public decimal? Bladepitch { get; set; }

    public decimal? Generatortemp { get; set; }

    public decimal? Gearboxtemp { get; set; }

    public decimal? Vibration { get; set; }

    public string Status { get; set; } = null!;
}
