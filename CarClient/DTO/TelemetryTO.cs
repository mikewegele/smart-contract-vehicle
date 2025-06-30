using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace CarClient.DTO;

public class TelemetryTO
{
    public required Point CurrentPosition { get; set; }
    public double CurrentSpeed { get; set; } // Speed in km/h
    public double Heading { get; set; } // Heading in degrees (0-360)
    public double RemainingReach { get; set; } // Remaining reach in km
}
