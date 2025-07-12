namespace SmartContractVehicle.DTO
{
    /// <summary>
    /// DTO for sending car connection status to the map view.
    /// </summary>
    public class CarConnectionStatusTO
    {
        public required string VIN { get; set; }
        public bool IsConnected { get; set; }
        public TelemetryTO? Telemetry { get; set; }
    }
}
