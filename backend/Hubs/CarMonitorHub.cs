// CarMonitorHub.cs
using Microsoft.AspNetCore.SignalR;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Hubs
{
    /// <summary>
    /// SignalR hub dedicated to broadcasting car state changes to the map view.
    /// </summary>
    public class CarMonitorHub : Hub
    {
        private readonly TelemetryService _telemetryService;

        public CarMonitorHub(TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// Called by a client (the Blazor map component) to get the initial state of all cars.
        /// The result is an IEnumerable of CarConnectionStatusTO.
        /// </summary>
        public async Task GetInitialCarStates()
        {
            var carStates = _telemetryService.GetAllCarStates();
            await Clients.Caller.SendAsync("InitialCarStates", carStates);
        }
    }
}
