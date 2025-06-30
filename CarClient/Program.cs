using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using CarClient.DTO;

namespace CarClient;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging(b => b.AddConfiguration(configuration.GetSection("Logging")).AddConsole())
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            var hubUrl = configuration["SignalR:HubUrl"];
            if (string.IsNullOrEmpty(hubUrl))
            {
                logger.LogError("HubUrl is not configured in appsettings.json.");
                return;
            }

            var vins = configuration.GetSection("Vins").Get<List<string>>();
            if (vins == null || !vins.Any())
            {
                logger.LogError("No VINs found in appsettings.json. Please add a 'Vins' array.");
                return;
            }

            var (wgs84ToUtm, utmToWgs84) = InitializeCoordinateSystems();

            logger.LogInformation("Starting {VinCount} car clients...", vins.Count);

            var cts = new CancellationTokenSource();
            var clientTasks = new List<Task>();

            foreach (var vin in vins)
            {
                var client = new CarClient(vin, hubUrl, wgs84ToUtm, utmToWgs84, loggerFactory);
                clientTasks.Add(client.StartAsync(cts.Token));
                await Task.Delay(100); // im overrunning the connection limit of postgres
            }

            logger.LogInformation("All clients are running. Press Enter to shut down.");
            Console.ReadLine();

            logger.LogInformation("Shutting down all clients...");
            cts.Cancel();
            await Task.WhenAll(clientTasks);
            logger.LogInformation("All clients have been shut down.");
        }

        private static (ICoordinateTransformation, ICoordinateTransformation) InitializeCoordinateSystems()
        {
            var csFactory = new CoordinateSystemFactory();
            var ctf = new CoordinateTransformationFactory();
            var wgs84 = csFactory.CreateFromWkt("GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]");
            var utm33n = csFactory.CreateFromWkt("PROJCS[\"WGS 84 / UTM zone 33N\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",15],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH],AUTHORITY[\"EPSG\",\"32633\"]]");
            var wgs84ToUtm = ctf.CreateFromCoordinateSystems(wgs84, utm33n);
            var utmToWgs84 = ctf.CreateFromCoordinateSystems(utm33n, wgs84);
            return (wgs84ToUtm, utmToWgs84);
        }
    }

    /// <summary>
    /// Represents a single car client, managing its own connection, state, and simulation logic.
    /// </summary>
    public class CarClient
    {
        private readonly ILogger<CarClient> _logger;
        private readonly string _vin;
        private readonly string _hubUrl;
        private readonly ICoordinateTransformation _wgs84ToUtm;
        private readonly ICoordinateTransformation _utmToWgs84;

        private HubConnection _connection;
        private TelemetryTO _currentTelemetry;
        private bool _isLocked = true;
        private const int UpdateIntervalMs = 2000;

        public CarClient(string vin, string hubUrl, ICoordinateTransformation wgs84ToUtm, ICoordinateTransformation utmToWgs84, ILoggerFactory loggerFactory)
        {
            _vin = vin;
            _hubUrl = hubUrl;
            _wgs84ToUtm = wgs84ToUtm;
            _utmToWgs84 = utmToWgs84;
            _logger = loggerFactory.CreateLogger<CarClient>();
        }

        public async Task StartAsync(CancellationToken token)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
                })
                .Build();

            SetupHubEventHandlers();

            try
            {
                if (await ConnectAndRegisterAsync(token))
                {
                    await Drive(token);
                }
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
                if (_currentTelemetry != null) _currentTelemetry.CurrentSpeed = 0;
                return true;
            });

            // **FIX**: This handler now preserves the client's heading on re-sync.
            _connection.On("RequestUnlockAndSync", (TelemetryTO serverState) =>
            {
                _logger.LogInformation("[{VIN}] Received UNLOCK AND SYNC request. Updating state from server.", _vin);

                // If the client already has telemetry data, preserve its heading. Otherwise, use the server's.
                double headingToKeep = (_currentTelemetry != null) ? _currentTelemetry.Heading : serverState.Heading;

                // Accept the new state from the server
                _currentTelemetry = serverState;

                // Restore the client's heading if it existed
                _currentTelemetry.Heading = headingToKeep;

                _isLocked = false;

                _logger.LogInformation("[{VIN}] State synchronized: Position=({Lat},{Lon}), Heading={Heading:F0}, Reach={Reach:F2} km. Car is UNLOCKED.",
                    _vin, _currentTelemetry.CurrentPosition.Y, _currentTelemetry.CurrentPosition.X, _currentTelemetry.Heading, _currentTelemetry.RemainingReach);
                return true;
            });
        }

        private async Task<bool> ConnectAndRegisterAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await _connection.StartAsync(token);
                    _logger.LogInformation("[{VIN}] Connection established with ID: {ConnectionId}", _vin, _connection.ConnectionId);
                    var registrationSuccess = await _connection.InvokeAsync<bool>("RegisterCar", _vin, token);
                    if (registrationSuccess)
                    {
                        _logger.LogInformation("[{VIN}] Client registered successfully.", _vin);
                        return true;
                    }
                    _logger.LogError("[{VIN}] Registration failed. Shutting down this client.", _vin);
                    return false;
                }
                catch (OperationCanceledException)
                {
                    // Let the exception propagate up to StartAsync to handle shutdown.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{VIN}] Failed to connect or register. Retrying in 5 seconds...", _vin);
                    await Task.Delay(5000, token);
                }
            }
            return false;
        }

        private async Task Drive(CancellationToken token)
        {
            var random = new Random();
            _logger.LogInformation("[{VIN}] Waiting for server to provide initial state...", _vin);

            while (!token.IsCancellationRequested)
            {
                if (_currentTelemetry != null)
                {
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
                return;
            }

            var currentWgs84Coord = new[] { _currentTelemetry.CurrentPosition.X, _currentTelemetry.CurrentPosition.Y };
            var currentUtmCoord = _wgs84ToUtm.MathTransform.Transform(currentWgs84Coord);

            var headingRad = _currentTelemetry.Heading * (Math.PI / 180.0);
            var dX = distanceMeters * Math.Sin(headingRad);
            var dY = distanceMeters * Math.Cos(headingRad);

            var newUtmCoord = new[] { currentUtmCoord[0] + dX, currentUtmCoord[1] + dY };
            var newWgs84Coord = _utmToWgs84.MathTransform.Transform(newUtmCoord);

            _currentTelemetry.CurrentPosition = new Point(newWgs84Coord[0], newWgs84Coord[1], 0) { SRID = 4326 };

            _logger.LogInformation("[{VIN}] Telemetry -> Speed: {Speed:F1} km/h, Heading: {Heading:F0}°, Pos: ({Lat:F4}, {Lon:F4}), Reach: {Reach:F2} km",
                _vin, _currentTelemetry.CurrentSpeed, _currentTelemetry.Heading, _currentTelemetry.CurrentPosition.Y, _currentTelemetry.CurrentPosition.X, _currentTelemetry.RemainingReach);
        }
    }