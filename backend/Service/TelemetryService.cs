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
        private readonly ConcurrentDictionary<string, TelemetryTO> _carStates = new();
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
        /// Announces to the dashboard that a car is connected, providing its last known location.
        /// </summary>
        public void SetCarConnected(string vin)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var car = dbContext.Cars.FirstOrDefault(c => c.VIN == vin);

            if (car == null)
            {
                _logger.LogWarning("Cannot announce connection for VIN {VIN}: car not found in DB.", vin);
                return;
            }

            // UPDATED: Use the car's position, or default to a known location (Berlin) if it's null.
            var initialPosition = car.CurrentPosition ?? new Point(13.4050, 52.5200) { SRID = 4326 };

            var initialTelemetry = new TelemetryTO
            {
                CurrentPosition = initialPosition,
                CurrentSpeed = 0,
                Heading =0,
                RemainingReach = car.RemainingReach
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
                var isConnected = _connectionMapping.GetConnectionId(car.VIN) != null;

                _carStates.TryGetValue(car.VIN, out var liveTelemetry);

                var dbPosition = car.CurrentPosition ?? new Point(13.4050, 52.5200) { SRID = 4326 };
                var telemetryForStatus = liveTelemetry ?? new TelemetryTO
                {
                    CurrentPosition = dbPosition,
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
