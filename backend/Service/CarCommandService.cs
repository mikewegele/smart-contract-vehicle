using Microsoft.AspNetCore.SignalR;
using SmartContractVehicle.Hubs;

namespace SmartContractVehicle.Service;

public interface ICarCommandService
{
    Task SendLockCommandAsync(string vin);
    Task SendUnlockCommandAsync(string vin);
}

public class CarCommandService : ICarCommandService
{
    private readonly IHubContext<CarHub> _hubContext;
    private readonly ILogger<CarCommandService> _logger;

    public CarCommandService(IHubContext<CarHub> hubContext, ILogger<CarCommandService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendLockCommandAsync(string vin)
    {
        _logger.LogInformation("Sending LOCK command to VIN: {VIN}", vin);
        // The core logic to send a "LockCommand" message to the group matching the VIN.
        await _hubContext.Clients.Group(vin).SendAsync("LockCommand");
    }

    public async Task SendUnlockCommandAsync(string vin)
    {
        _logger.LogInformation("Sending UNLOCK command to VIN: {VIN}", vin);
        // The core logic to send an "UnlockCommand" message to the group matching the VIN.
        await _hubContext.Clients.Group(vin).SendAsync("UnlockCommand");
    }
}

