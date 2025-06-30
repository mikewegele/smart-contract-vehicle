using Microsoft.AspNetCore.SignalR;
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
        private readonly ConcurrentDictionary<string, TelemetryTO> _carStates = new();

        public TelemetryService(
            ILogger<TelemetryService> logger,
            IHubContext<CarMonitorHub> monitorHubContext,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _monitorHubContext = monitorHubContext;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Announces to the dashboard that a car is connected, providing its last known location.
        /// </summary>
        public void SetCarConnected(string vin)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Find the car in the database to get its last known state.
            var car = dbContext.Cars.FirstOrDefault(c => c.VIN == vin);

            // This shouldn't happen because RegisterCar validates existence, but as a safeguard:
            if (car == null || car.CurrentPosition == null)
            {
                _logger.LogWarning("Could not announce initial position for VIN {VIN}: car or its position not found in DB.", vin);
                // Announce connection status so the list updates, even if the map can't.
                var statusOnly = new CarConnectionStatusTO { VIN = vin, IsConnected = true, Telemetry = null };
                _monitorHubContext.Clients.All.SendAsync("CarStateChanged", statusOnly);
                return;
            }

            // Create a telemetry object from the car's last known data in the database.
            var initialTelemetry = new TelemetryTO
            {
                CurrentPosition = car.CurrentPosition,
                CurrentSpeed = 0, // Car is stationary on connect
                Heading = 0,
                RemainingReach = car.RemainingReach // Assumes a 'RemainingReach' property
            };

            var carStatus = new CarConnectionStatusTO
            {
                VIN = vin,
                IsConnected = true,
                Telemetry = initialTelemetry
            };

            _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            _logger.LogInformation("Announced connection for VIN: {VIN} with initial position.", vin);
        }

        public void UpdateCarState(string vin, TelemetryTO newState)
        {
            _carStates[vin] = newState;
            var carStatus = new CarConnectionStatusTO
            {
                VIN = vin,
                IsConnected = true,
                Telemetry = newState
            };
            _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
        }

        public void RemoveCarState(string vin)
        {
            if (_carStates.TryRemove(vin, out var lastKnownState))
            {
                _logger.LogInformation("Removed state for disconnected VIN: {VIN}", vin);
                var carStatus = new CarConnectionStatusTO
                {
                    VIN = vin,
                    IsConnected = false,
                    Telemetry = lastKnownState
                };
                _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            }
        }

        public TelemetryTO? GetCarState(string vin)
        {
            _carStates.TryGetValue(vin, out var state);
            return state;
        }

        public IEnumerable<CarConnectionStatusTO> GetAllCarStates()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var allCars = dbContext.Cars.ToList();

            var carStatuses = allCars.Select(car =>
            {
                var isConnected = _carStates.TryGetValue(car.VIN, out var liveTelemetry);

                // Use live telemetry if available, otherwise use last known data from DB.
                var telemetryForStatus = liveTelemetry ?? new TelemetryTO
                {
                    CurrentPosition = car.CurrentPosition,
                    CurrentSpeed = 0,
                    Heading = 0,
                    RemainingReach = car.RemainingReach
                };

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
