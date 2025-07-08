using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;
using CarClient.DTO;
using Microsoft.Extensions.DependencyInjection;

namespace CarClient;

/// <summary>
/// Represents a single car client, managing its own connection, state, and simulation logic.
/// </summary>
public class CarClient(string vin, string hubUrl, ICoordinateTransformation wgs84ToUtm, ICoordinateTransformation utmToWgs84, ILoggerFactory loggerFactory)
{
    private readonly ILogger<CarClient> _logger = loggerFactory.CreateLogger<CarClient>();
    private readonly string _vin = vin;
    private readonly string _baseHubUrl = hubUrl; // Renamed to clarify it's the base URL
    private readonly ICoordinateTransformation _wgs84ToUtm = wgs84ToUtm;
    private readonly ICoordinateTransformation _utmToWgs84 = utmToWgs84;

    private HubConnection _connection;
    private TelemetryTO _currentTelemetry;
    private bool _isLocked = true; // Initial state: car is locked
    private const int UpdateIntervalMs = 2000;

    public async Task StartAsync(CancellationToken token)
    {
        // Construct the full URL with VIN query parameter for server-side registration
        var fullHubUrl = $"{_baseHubUrl}?vin={_vin}";

        _connection = new HubConnectionBuilder()
            .WithUrl(fullHubUrl) // Use the full URL with VIN here
            .WithAutomaticReconnect()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
            })
            .Build();

        SetupHubEventHandlers();

        try
        {

            await ConnectAndStartDriving(token);
        }
        catch (OperationCanceledException)
        {
            // This is expected on a clean shutdown.
            _logger.LogInformation("[{VIN}] Shutdown requested.", _vin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{VIN}] An unhandled exception occurred in the client task.", _vin);
        }
        finally
        {
            // Ensure disconnection is logged and called explicitly on exit.
            if (_connection != null)
            {
                _logger.LogInformation("[{VIN}] Disconnecting from the server...", _vin);
                await _connection.DisposeAsync();
            }
        }
    }

    private void SetupHubEventHandlers()
    {
        _connection.On("RequestLock", () =>
        {
            _logger.LogInformation("[{VIN}] Received LOCK request. Stopping car.", _vin);
            _isLocked = true;
            if (_currentTelemetry != null)
            {
                _currentTelemetry.CurrentSpeed = 0;
                _currentTelemetry.IsLocked = true; // Update TelemetryTO with locked status
            }
            return true;
        });

        // This handler now preserves the client's heading on re-sync.
        _connection.On("RequestUnlockAndSync", (TelemetryTO serverState) =>
        {
            _logger.LogInformation("[{VIN}] Received UNLOCK AND SYNC request. Updating state from server.", _vin);

            // If the client already has telemetry data, preserve its heading. Otherwise, use the server's.
            double headingToKeep = (_currentTelemetry != null) ? _currentTelemetry.Heading : serverState.Heading;

            // Accept the new state from the server
            _currentTelemetry = serverState;

            // Restore the client's heading if it existed
            _currentTelemetry.Heading = headingToKeep;
            _currentTelemetry.IsLocked = false; // Update TelemetryTO with unlocked status

            _isLocked = false;

            _logger.LogInformation("[{VIN}] State synchronized: Position=({Lat},{Lon}), Heading={Heading:F0}, Reach={Reach:F2} km. Car is UNLOCKED.",
                _vin, _currentTelemetry.CurrentPosition.Y, _currentTelemetry.CurrentPosition.X, _currentTelemetry.Heading, _currentTelemetry.RemainingReach);
            return true;
        });
    }

    /// <summary>
    /// Handles the initial connection attempt and retries.
    /// The server's OnConnectedAsync now performs the registration based on the VIN in the query string.
    /// </summary>
    private async Task ConnectAndStartDriving(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await _connection.StartAsync(token);
                _logger.LogInformation("[{VIN}] Connection established and registered with ID: {ConnectionId}", _vin, _connection.ConnectionId);
                // If StartAsync succeeds, the car is considered registered by the server.
                await Drive(token); // Start driving once connected
                return; // Exit loop if successfully connected and driving
            }
            catch (OperationCanceledException)
            {
                // Let the exception propagate up to StartAsync to handle shutdown.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{VIN}] Failed to connect. Retrying in 5 seconds...", _vin);
                await Task.Delay(5000, token);
            }
        }
    }

    private async Task Drive(CancellationToken token)
    {
        var random = new Random();
        _logger.LogInformation("[{VIN}] Waiting for server to provide initial state...", _vin);

        while (!token.IsCancellationRequested)
        {
            if (_currentTelemetry != null)
            {
                // Ensure _currentTelemetry.IsLocked always reflects the internal _isLocked state
                _currentTelemetry.IsLocked = _isLocked;

                if (!_isLocked)
                {
                    UpdateSimulation(random);
                }

                try
                {
                    await _connection.SendAsync("UpdateCarState", _currentTelemetry, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{VIN}] Failed to send telemetry update.", _vin);
                }
            }
            await Task.Delay(UpdateIntervalMs, token);
        }
    }

    private void UpdateSimulation(Random random)
    {
        if (_currentTelemetry == null) return;

        if (random.Next(10) > 4) _currentTelemetry.CurrentSpeed += random.Next(-50, 50);
        if (_currentTelemetry.CurrentSpeed < 0) _currentTelemetry.CurrentSpeed = 0;
        if (_currentTelemetry.CurrentSpeed > 180) _currentTelemetry.CurrentSpeed = 180;

        if (random.Next(10) > 5) _currentTelemetry.Heading += random.Next(-10, 11);
        _currentTelemetry.Heading = (_currentTelemetry.Heading + 360) % 360;

        var speedMetersPerSecond = _currentTelemetry.CurrentSpeed * 1000 / 3600;
        var distanceMeters = speedMetersPerSecond * (UpdateIntervalMs / 1000.0);

        _currentTelemetry.RemainingReach -= distanceMeters / 1000.0;
        if (_currentTelemetry.RemainingReach <= 0)
        {
            _currentTelemetry.CurrentSpeed = 0;
            _currentTelemetry.RemainingReach = 0;
            // When reach is 0, the car should effectively be considered "stopped" and potentially "locked"
            // This is a design choice; for now, we'll just stop it.
        }

        var currentWgs84Coord = new[] { _currentTelemetry.CurrentPosition.X, _currentTelemetry.CurrentPosition.Y };
        var currentUtmCoord = _wgs84ToUtm.MathTransform.Transform(currentWgs84Coord);

        var headingRad = _currentTelemetry.Heading * (Math.PI / 180.0);
        var dX = distanceMeters * Math.Sin(headingRad);
        var dY = distanceMeters * Math.Cos(headingRad);

        var newUtmCoord = new[] { currentUtmCoord[0] + dX, currentUtmCoord[1] + dY };
        var newWgs84Coord = _utmToWgs84.MathTransform.Transform(newUtmCoord);

        _currentTelemetry.CurrentPosition = new Point(newWgs84Coord[0], newWgs84Coord[1], 0) { SRID = 4326 };

        _logger.LogInformation("[{VIN}] Telemetry -> Speed: {Speed:F1} km/h, Heading: {Heading:F0}°, Pos: ({Lat:F4}, {Lon:F4}), Reach: {Reach:F2} km, Locked: {IsLocked}",
            _vin, _currentTelemetry.CurrentSpeed, _currentTelemetry.Heading, _currentTelemetry.CurrentPosition.Y, _currentTelemetry.CurrentPosition.X, _currentTelemetry.RemainingReach, _currentTelemetry.IsLocked);
    }
}
