using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Geometries;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Hubs;
using System.Collections.Concurrent;

namespace SmartContractVehicle.Service
{
    public class TelemetryService
    {
        private readonly ILogger<TelemetryService> _logger;
        private readonly IHubContext<CarMonitorHub> _monitorHubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        // This dictionary stores the LATEST telemetry received from connected cars.
        // It should NOT be used to store disconnected car states or their last known telemetry
        // after they disconnect, as the source of truth for disconnected cars' last state
        // is the database. For connected cars, it's the live telemetry.
        private readonly ConcurrentDictionary<string, TelemetryTO> _liveCarTelemetries = new();
        private readonly ConnectionMapping _connectionMapping;

        public TelemetryService(
            ILogger<TelemetryService> logger,
            IHubContext<CarMonitorHub> monitorHubContext,
            IServiceScopeFactory scopeFactory,
            ConnectionMapping connectionMapping)
        {
            _logger = logger;
            _monitorHubContext = monitorHubContext;
            _scopeFactory = scopeFactory;
            _connectionMapping = connectionMapping;
        }

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
            var initialTelemetry = new TelemetryTO
            {
                CurrentPosition = initialPosition,
                CurrentSpeed = 0, // Assume 0 speed on initial connection
                Heading = 0,      // Assume 0 heading on initial connection
                RemainingReach = car.RemainingReach // Use remaining reach from DB
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
            _logger.LogInformation("Announced connection for VIN: {VIN} with initial position.", vin);
        }

        /// <summary>
        /// Updates the live telemetry for a car and broadcasts the change to the dashboard.
        /// </summary>
        public async void UpdateCarState(string vin, TelemetryTO newState)
        {
            // Update the live telemetry in the concurrent dictionary.
            _liveCarTelemetries.AddOrUpdate(vin, newState, (key, existingVal) => newState);

            var carStatus = new CarConnectionStatusTO
            {
                VIN = vin,
                IsConnected = true, // A car sending updates is by definition connected
                Telemetry = newState // Send the latest telemetry
            };

            // Broadcast the updated state to all dashboard clients.
            await _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            _logger.LogInformation("Updated state for VIN: {VIN}.", vin);
        }

        /// <summary>
        /// Removes a car's live state and broadcasts its disconnected status to the dashboard.
        /// </summary>
        public async void RemoveCarState(string vin)
        {
            // Try to remove the live telemetry from the dictionary.
            if (_liveCarTelemetries.TryRemove(vin, out _)) // We don't need the lastKnownState here for the broadcast
            {
                _logger.LogInformation("Removed live state for disconnected VIN: {VIN}", vin);

                // Create a CarConnectionStatusTO indicating disconnection and NO live telemetry.
                // Setting Telemetry to null is crucial for the frontend to remove the marker.
                var carStatus = new CarConnectionStatusTO
                {
                    VIN = vin,
                    IsConnected = false,
                    Telemetry = null // Explicitly set telemetry to null for disconnected cars
                };

                // Broadcast the disconnected status to all dashboard clients.
                await _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            }
            else
            {
                _logger.LogWarning("Attempted to remove state for VIN {VIN}, but it was not found in live states.", vin);
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

                TelemetryTO? telemetryForStatus = null;

                if (isConnected)
                {
                    // If connected, try to get the live telemetry.
                    // If no live telemetry yet (e.g., just connected but no UpdateCarState received),
                    // use initial data from DB or default.
                    if (_liveCarTelemetries.TryGetValue(car.VIN, out var liveTelemetry))
                    {
                        telemetryForStatus = liveTelemetry;
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
                            RemainingReach = car.RemainingReach
                        };
                    }
                }
                // If not connected, telemetryForStatus remains null, which is what the frontend expects
                // to remove the marker and display '--' for telemetry fields.

                return new CarConnectionStatusTO
                {
                    VIN = car.VIN,
                    IsConnected = isConnected,
                    Telemetry = telemetryForStatus // Will be null if disconnected, or live data if connected
                };
            }).ToList();

            return carStatuses;
        }
    }
}
