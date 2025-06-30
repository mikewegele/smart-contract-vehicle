using Microsoft.AspNetCore.SignalR;
using SmartContractVehicle.Hubs;

namespace SmartContractVehicle.Service;

// A standard response object for command results
public class CarCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public interface ICarCommandService
{
    Task<CarCommandResponse> SendLockCommandAsync(string vin);
    Task<CarCommandResponse> SendUnlockCommandAsync(string vin);
}

public class CarCommandService : ICarCommandService
{
    private readonly IHubContext<CarHub> _hubContext;
    private readonly ILogger<CarCommandService> _logger;
    private readonly ConnectionMapping _connectionMapping;

    public CarCommandService(IHubContext<CarHub> hubContext, ILogger<CarCommandService> logger, ConnectionMapping connectionMapping)
    {
        _hubContext = hubContext;
        _logger = logger;
        _connectionMapping = connectionMapping;
    }

    public async Task<CarCommandResponse> SendLockCommandAsync(string vin)
    {
        return await SendCommandAndWaitForResponse(vin, "RequestLock");
    }

    public async Task<CarCommandResponse> SendUnlockCommandAsync(string vin)
    {
        return await SendCommandAndWaitForResponse(vin, "RequestUnlock");
    }

    private async Task<CarCommandResponse> SendCommandAndWaitForResponse(string vin, string command)
    {
        var connectionId = _connectionMapping.GetConnectionId(vin);
        if (connectionId == null)
        {
            _logger.LogWarning("Attempted to send '{Command}' to VIN {VIN}, but it is offline.", command, vin);
            return new CarCommandResponse { Success = false, Message = "Car is offline." };
        }

        _logger.LogInformation("Sending '{Command}' to VIN {VIN} on connection {ConnectionId}", command, vin, connectionId);

        try
        {
            // Send the command and wait for a boolean response from the client
            var clientResponse = await _hubContext.Clients.Client(connectionId).InvokeAsync<bool>(command, new CancellationTokenSource(5000).Token);

            if (clientResponse)
            {
                _logger.LogInformation("VIN {VIN} successfully executed '{Command}'.", vin, command);
                return new CarCommandResponse { Success = true, Message = "Command executed successfully." };
            }
            else
            {
                _logger.LogError("VIN {VIN} failed to execute '{Command}'.", vin, command);
                return new CarCommandResponse { Success = false, Message = "Car failed to execute command." };
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Request to VIN {VIN} for '{Command}' timed out.", vin, command);
            return new CarCommandResponse { Success = false, Message = "Request timed out. Car did not respond." };
        }
    }
}
