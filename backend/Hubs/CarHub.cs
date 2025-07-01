using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Hubs
{
    public class CarHub : Hub
    {
        private readonly ILogger<CarHub> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionMapping _connectionMapping;
        private readonly TelemetryService _telemetryService;

        public CarHub(
            ILogger<CarHub> logger,
            IServiceScopeFactory scopeFactory,
            ConnectionMapping connectionMapping,
            TelemetryService telemetryService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectionMapping = connectionMapping;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// Called by the car client to push its latest driving telemetry to the server.
        /// </summary>
        public void UpdateCarState(TelemetryTO telemetry)
        {
            var vin = _connectionMapping.GetVin(Context.ConnectionId);
            if (vin != null)
            {
                _telemetryService.UpdateCarState(vin, telemetry);
            }
            else
            {
                _logger.LogWarning("Received a state update from an unregistered connection: {ConnectionId}", Context.ConnectionId);
            }
        }

        /// <summary>
        /// This method now handles the registration atomically when a car connects.
        /// The car MUST provide its VIN in the query string of the connection URL.
        /// e.g., "https://yourserver.com/carhub?vin=VIN123"
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var vin = Context.GetHttpContext()?.Request.Query["vin"].ToString();

            if (string.IsNullOrEmpty(vin))
            {
                _logger.LogWarning("A client with connection ID {ConnectionId} attempted to connect without a VIN.", Context.ConnectionId);
                Context.Abort(); // Abort connection if VIN is missing
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var carExists = await dbContext.Cars.AnyAsync(c => c.VIN == vin);
                if (!carExists)
                {
                    _logger.LogWarning("Client {ConnectionId} attempted to connect with a non-existent VIN: {VIN}.", Context.ConnectionId, vin);
                    Context.Abort(); // Abort connection if VIN does not exist in DB
                    return;
                }
            }

            _connectionMapping.Add(vin, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, vin);

            // Announce to the dashboard that this car is now connected.
            _telemetryService.SetCarConnected(vin);

            _logger.LogInformation("Car with VIN {VIN} connected and registered with connection ID {ConnectionId}.", vin, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var vin = _connectionMapping.GetVin(Context.ConnectionId);
            if (vin != null)
            {
                _connectionMapping.Remove(vin);
                _telemetryService.RemoveCarState(vin); // This notifies the dashboard
                _logger.LogInformation("Client for VIN {VIN} disconnected.", vin);
            }

            if (exception != null)
            {
                _logger.LogError(exception, "Client {ConnectionId} disconnected with an error.", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
