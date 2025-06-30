using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartContractVehicle.Data;
using System;
using System.Threading.Tasks;

namespace SmartContractVehicle.Hubs
{
    public class CarHub : Hub
    {
        private readonly ILogger<CarHub> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public CarHub(ILogger<CarHub> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        // This method will be called by the client immediately after connecting.
        // It validates the VIN and returns a boolean indicating if registration was successful.
        public async Task<bool> RegisterCar(string vin)
        {
            if (string.IsNullOrEmpty(vin))
            {
                _logger.LogWarning("A client with connection ID {ConnectionId} attempted to register with an empty VIN.", Context.ConnectionId);
                return false;
            }

            // Create a new scope to resolve the DbContext, which is the correct pattern for hubs.
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var carExists = await dbContext.Cars.AnyAsync(c => c.VIN == vin);

                if (!carExists)
                {
                    _logger.LogWarning("Client {ConnectionId} attempted to register with a non-existent VIN: {VIN}.", Context.ConnectionId, vin);
                    return false; // Indicate failure to the client
                }
            }

            // Add the connection to a group named after the VIN.
            // This allows us to send messages specifically to this car.
            await Groups.AddToGroupAsync(Context.ConnectionId, vin);
            _logger.LogInformation("Car client with VIN {VIN} connected and registered with connection ID {ConnectionId}.", vin, Context.ConnectionId);
            return true; // Indicate success
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("A new client connected: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        // SignalR automatically handles removing connections from groups when they disconnect.
        // You can override OnDisconnectedAsync for custom logging or logic if needed.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogError(exception, "Client {ConnectionId} disconnected with an error.", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client {ConnectionId} disconnected.", Context.ConnectionId);
            }
            // The connection is automatically removed from any groups.
            await base.OnDisconnectedAsync(exception);
        }
    }
}
