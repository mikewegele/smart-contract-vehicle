using SmartContractVehicle.DTO;
using System.Collections.Concurrent;

namespace SmartContractVehicle.Service;

// This service holds the real-time state of all active cars.
// It is registered as a Singleton to maintain state across requests.
public class TelemetryService(ILogger<TelemetryService> logger)
{
    private readonly ILogger<TelemetryService> _logger = logger;

    // A thread-safe dictionary to store the latest telemetry for each VIN.
    private readonly ConcurrentDictionary<string, TelemetryTO> _carStates = new();

    /// <summary>
    /// Updates the telemetry for a given car VIN.
    /// </summary>
    public void UpdateCarState(string vin, TelemetryTO newState)
    {
        _carStates[vin] = newState;
    }

    /// <summary>
    /// Retrieves the current telemetry for a given car VIN.
    /// </summary>
    /// <returns>The car's telemetry DTO, or null if the car is not currently reporting its state.</returns>
    public TelemetryTO? GetCarState(string vin)
    {
        _carStates.TryGetValue(vin, out var state);
        return state;
    }

    /// <summary>
    /// Removes the telemetry for a given car VIN, e.g., when it disconnects.
    /// </summary>
    public void RemoveCarState(string vin)
    {
        if (_carStates.TryRemove(vin, out _))
        {
            _logger.LogInformation("Removed state for disconnected VIN: {VIN}", vin);
        }
    }
}
