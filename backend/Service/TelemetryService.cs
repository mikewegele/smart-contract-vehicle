// TelemetryService.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SmartContractVehicle.Data; // Ensure this using statement points to your DbContext
using SmartContractVehicle.DTO;
using SmartContractVehicle.Hubs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
        /// Announces to the dashboard that a car is connected, without waiting for telemetry.
        /// </summary>
        /// <param name="vin">The Vehicle Identification Number of the connected car.</param>
        public void SetCarConnected(string vin)
        {
            var carStatus = new CarConnectionStatusTO
            {
                VIN = vin,
                IsConnected = true,
                Telemetry = null // No telemetry data is available on initial connection.
            };
            _monitorHubContext.Clients.All.SendAsync("CarStateChanged", carStatus);
            _logger.LogInformation("Announced connection for VIN: {VIN}", vin);
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

        /// <summary>
        /// Retrieves the current telemetry for a given car VIN from the in-memory store.
        /// </summary>
        /// <returns>The car's telemetry DTO, or null if the car is not currently reporting its state.</returns>
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
                var isConnected = _carStates.TryGetValue(car.VIN, out var telemetry);
                return new CarConnectionStatusTO
                {
                    VIN = car.VIN,
                    IsConnected = isConnected,
                    Telemetry = telemetry
                };
            }).ToList();

            return carStatuses;
        }
    }
}
