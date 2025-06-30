using Microsoft.AspNetCore.SignalR;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Hubs;
namespace SmartContractVehicle.Service;

// A standard response object for command results
public class CarCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CarCommandService
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

    public async Task<CarCommandResponse> SendUnlockAndSyncCommandAsync(string vin, TelemetryTO initialState)
    {
        var connectionId = _connectionMapping.GetConnectionId(vin);
        if (connectionId == null)
        {
            _logger.LogWarning("Attempted to send 'RequestUnlockAndSync' to VIN {VIN}, but it is offline.", vin);
            return new CarCommandResponse { Success = false, Message = "Car is offline." };
        }

        _logger.LogInformation("Sending 'RequestUnlockAndSync' to VIN {VIN} on connection {ConnectionId}", vin, connectionId);

        try
        {
            // Explicitly pass the TelemetryTO object as the argument.
            // SignalR will serialize this object and the client handler will receive it as a single parameter.
            var clientResponse = await _hubContext.Clients.Client(connectionId).InvokeAsync<bool>(
                "RequestUnlockAndSync",
                initialState, // The strongly-typed object
                new System.Threading.CancellationTokenSource(5000).Token);

            if (clientResponse)
            {
                _logger.LogInformation("VIN {VIN} successfully executed 'RequestUnlockAndSync'.", vin);
                return new CarCommandResponse { Success = true, Message = "Command executed successfully." };
            }
            else
            {
                _logger.LogError("VIN {VIN} failed to execute 'RequestUnlockAndSync'.", vin);
                return new CarCommandResponse { Success = false, Message = "Car failed to execute command." };
            }
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            _logger.LogError("Request to VIN {VIN} for 'RequestUnlockAndSync' timed out.", vin);
            return new CarCommandResponse { Success = false, Message = "Request timed out. Car did not respond." };
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while sending 'RequestUnlockAndSync' to VIN {VIN}", vin);
            return new CarCommandResponse { Success = false, Message = "An unexpected server error occurred." };
        }
    }

    /// <summary>
    /// A helper method to send a command without arguments to a specific client and wait for a boolean response.
    /// </summary>
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
            var clientResponse = await _hubContext.Clients.Client(connectionId).InvokeAsync<bool>(command, new System.Threading.CancellationTokenSource(5000).Token);

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
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            _logger.LogError("Request to VIN {VIN} for '{Command}' timed out.", vin, command);
            return new CarCommandResponse { Success = false, Message = "Request timed out. Car did not respond." };
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while sending '{Command}' to VIN {VIN}", command, vin);
            return new CarCommandResponse { Success = false, Message = "An unexpected server error occurred." };
        }
    }
}
