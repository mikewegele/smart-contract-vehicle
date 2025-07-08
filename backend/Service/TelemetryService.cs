using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Geometries;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Hubs;
using System.Collections.Concurrent;

namespace SmartContractVehicle.Service
{
    public class TelemetryService(
        ILogger<TelemetryService> logger,
        IHubContext<CarMonitorHub> monitorHubContext,
        IServiceScopeFactory scopeFactory,
        ConnectionMappingService connectionMapping)
    {
        private readonly ILogger<TelemetryService> _logger = logger;
        private readonly IHubContext<CarMonitorHub> _monitorHubContext = monitorHubContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        // This dictionary stores the LATEST telemetry received from connected cars.
        // It should NOT be used to store disconnected car states or their last known telemetry
        // after they disconnect, as the source of truth for disconnected cars' last state
        // is the database. For connected cars, it's the live telemetry.
        private readonly ConcurrentDictionary<string, TelemetryTO> _liveCarTelemetries = new();
        private readonly ConnectionMappingService _connectionMapping = connectionMapping;

        /// <summary>
        /// Announces to the dashboard that a car is connected, providing its initial state.
        /// This state might be retrieved from the database if no live telemetry exists yet.
        /// </summary>
        public async void SetCarConnected(string vin)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var car = dbContext.Cars.FirstOrDefault(c => c.VIN == vin);

            if (car == null)
            {
                _logger.LogWarning("Cannot announce connection for VIN {VIN}: car not found in DB.", vin);
                return;
            }

            // Get the last known position from the database, or default to Berlin if not available.
            var initialPosition = car.CurrentPosition ?? new Point(13.4050, 52.5200) { SRID = 4326 };

            // Create an initial TelemetryTO based on DB data, or a default if no live data has been received yet.
            // This will be the initial state sent to the dashboard.
            // When a car first connects, it is assumed to be locked until explicitly unlocked.
            var initialTelemetry = new TelemetryTO
            {
                CurrentPosition = initialPosition,
                CurrentSpeed = 0, // Assume 0 speed on initial connection
                Heading = 0,      // Assume 0 heading on initial connection
                RemainingReach = car.RemainingReach, // Use remaining reach from DB
                IsLocked = true // Initial state for a newly connected car is locked
            };

            // Store this initial telemetry as the live state for this VIN.
            _liveCarTelemetries.AddOrUpdate(vin, initialTelemetry, (key, existingVal) => initialTelemetry);

            var carStatus = new CarConnectionStatusTO
            {
                VIN = vin,
                IsConnected = true,
                Telemetry = initialTelemetry // Send the initial telemetry
            };

            // Broadcast the connected status with initial telemetry to all dashboard clients.
            await _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            _logger.LogInformation("Announced connection for VIN: {VIN} with initial position and locked status.", vin);
        }

        /// <summary>
        /// Updates the live telemetry for a car and broadcasts the change to the dashboard.
        /// </summary>
        public async void UpdateCarState(string vin, TelemetryTO newState)
        {
            // Update the live telemetry in the concurrent dictionary.
            // The newState already contains the IsLocked status from the client.
            _liveCarTelemetries.AddOrUpdate(vin, newState, (key, existingVal) => newState);

            var carStatus = new CarConnectionStatusTO
            {
                VIN = vin,
                IsConnected = true, // A car sending updates is by definition connected
                Telemetry = newState // Send the latest telemetry, including IsLocked
            };

            // Broadcast the updated state to all dashboard clients.
            await _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            _logger.LogInformation("Updated state for VIN: {VIN}. Speed: {Speed}, Locked: {IsLocked}", vin, newState.CurrentSpeed, newState.IsLocked);
        }

        /// <summary>
        /// Removes a car's live state and broadcasts its disconnected status to the dashboard.
        /// </summary>
        public async void RemoveCarState(string vin)
        {
            // Try to remove the live telemetry from the dictionary.
            if (_liveCarTelemetries.TryRemove(vin, out var lastKnownState)) // Capture lastKnownState
            {
                _logger.LogInformation("Removed live state for disconnected VIN: {VIN}", vin);

                // Create a CarConnectionStatusTO indicating disconnection.
                // Use the last known telemetry (which includes position and IsLocked) so the marker stays on the map.
                var carStatus = new CarConnectionStatusTO
                {
                    VIN = vin,
                    IsConnected = false,
                    Telemetry = lastKnownState // Keep last known telemetry for display on map
                };

                // Broadcast the disconnected status to all dashboard clients.
                await _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            }
            else
            {
                _logger.LogWarning("Attempted to remove state for VIN {VIN}, but it was not found in live states. Fetching from DB as fallback.", vin);
                // Even if not found in live states, if it was in connection mapping, it means it was connected.
                // We should still send a disconnected status using DB data as fallback.
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var car = dbContext.Cars.FirstOrDefault(c => c.VIN == vin);

                if (car != null)
                {
                    var lastKnownPosition = car.CurrentPosition ?? new Point(13.4050, 52.5200) { SRID = 4326 };
                    var defaultTelemetry = new TelemetryTO
                    {
                        CurrentPosition = lastKnownPosition,
                        CurrentSpeed = 0,
                        Heading = 0,
                        RemainingReach = car.RemainingReach,
                        IsLocked = true // Assume disconnected cars are locked by default for display
                    };
                    var carStatus = new CarConnectionStatusTO
                    {
                        VIN = vin,
                        IsConnected = false,
                        Telemetry = defaultTelemetry
                    };
                    await _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
                }
            }
        }

        /// <summary>
        /// Gets the current live telemetry for a specific car.
        /// </summary>
        public TelemetryTO? GetCarState(string vin)
        {
            _liveCarTelemetries.TryGetValue(vin, out var state);
            return state;
        }

        /// <summary>
        /// Retrieves the current connection and telemetry status for all cars.
        /// This is used for initial dashboard load.
        /// </summary>
        public IEnumerable<CarConnectionStatusTO> GetAllCarStates()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var allCars = dbContext.Cars.ToList();

            var carStatuses = allCars.Select(car =>
            {
                // Determine if the car is currently connected based on ConnectionMapping.
                var isConnected = _connectionMapping.GetConnectionId(car.VIN) != null;
                TelemetryTO telemetryForStatus;

                if (isConnected)
                {
                    // If connected, try to get the live telemetry.
                    if (_liveCarTelemetries.TryGetValue(car.VIN, out var liveTelemetry))
                    {
                        telemetryForStatus = liveTelemetry; // Use live telemetry, which includes IsLocked
                    }
                    else
                    {
                        // Fallback for newly connected cars that haven't sent telemetry yet
                        var initialPosition = car.CurrentPosition ?? new Point(13.4050, 52.5200) { SRID = 4326 };
                        telemetryForStatus = new TelemetryTO
                        {
                            CurrentPosition = initialPosition,
                            CurrentSpeed = 0,
                            Heading = 0,
                            RemainingReach = car.RemainingReach,
                            IsLocked = true // Assume new connections start locked
                        };
                    }
                }
                else // If not connected
                {
                    // For disconnected cars, use their last known position from the DB
                    // and set speed/heading to 0. Assume they are locked when disconnected.
                    var lastKnownPosition = car.CurrentPosition ?? new Point(13.4050, 52.5200) { SRID = 4326 };
                    telemetryForStatus = new TelemetryTO
                    {
                        CurrentPosition = lastKnownPosition,
                        CurrentSpeed = 0,
                        Heading = 0,
                        RemainingReach = car.RemainingReach,
                        IsLocked = true // Assume disconnected cars are locked
                    };
                }

                return new CarConnectionStatusTO
                {
                    VIN = car.VIN,
                    IsConnected = isConnected,
                    Telemetry = telemetryForStatus
                };
            }).ToList();

            return carStatuses;
        }
    }
}
