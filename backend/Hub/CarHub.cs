using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartContractVehicle.Data;
using SmartContractVehicle.Service;
using System;
using System.Threading.Tasks;

namespace SmartContractVehicle.Hubs
{
    public class CarHub : Hub
    {
        private readonly ILogger<CarHub> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionMapping _connectionMapping;

        public CarHub(ILogger<CarHub> logger, IServiceScopeFactory scopeFactory, ConnectionMapping connectionMapping)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectionMapping = connectionMapping;
        }

        public async Task<bool> RegisterCar(string vin)
        {
            if (string.IsNullOrEmpty(vin))
            {
                _logger.LogWarning("A client with connection ID {ConnectionId} attempted to register with an empty VIN.", Context.ConnectionId);
                return false;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var carExists = await dbContext.Cars.AnyAsync(c => c.VIN == vin);
                if (!carExists)
                {
                    _logger.LogWarning("Client {ConnectionId} attempted to register with a non-existent VIN: {VIN}.", Context.ConnectionId, vin);
                    return false;
                }
            }

            // Track the connection
            _connectionMapping.Add(vin, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, vin);
            _logger.LogInformation("Car client with VIN {VIN} connected and registered with connection ID {ConnectionId}.", vin, Context.ConnectionId);
            return true;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Find the VIN for the disconnecting client and remove it from tracking
            var vin = _connectionMapping.GetVin(Context.ConnectionId);
            if (vin != null)
            {
                _connectionMapping.Remove(vin);
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
