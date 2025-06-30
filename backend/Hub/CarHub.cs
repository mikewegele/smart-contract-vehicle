using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Service;
using System;
using System.Threading.Tasks;

namespace SmartContractVehicle.Hubs;

public class CarHub(
    ILogger<CarHub> logger,
    IServiceScopeFactory scopeFactory,
    ConnectionMapping connectionMapping,
    TelemetryService carStateService) : Hub
{
    private readonly ILogger<CarHub> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ConnectionMapping _connectionMapping = connectionMapping;
    private readonly TelemetryService _ts = carStateService;

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

        _connectionMapping.Add(vin, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, vin);
        _logger.LogInformation("Car client with VIN {VIN} connected and registered with connection ID {ConnectionId}.", vin, Context.ConnectionId);
        return true;
    }

    /// <summary>
    /// Called by the car client to push its latest driving telemetry to the server.
    /// </summary>
    public void UpdateCarState(TelemetryTO telemetry)
    {
        var vin = _connectionMapping.GetVin(Context.ConnectionId);
        if (vin != null)
        {
            _ts.UpdateCarState(vin, telemetry);
        }
        else
        {
            _logger.LogWarning("Received a state update from an unregistered connection: {ConnectionId}", Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var vin = _connectionMapping.GetVin(Context.ConnectionId);
        if (vin != null)
        {
            _connectionMapping.Remove(vin);
            _ts.RemoveCarState(vin); // Clean up the state
            _logger.LogInformation("Client for VIN {VIN} disconnected.", vin);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Client {ConnectionId} disconnected with an error.", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
