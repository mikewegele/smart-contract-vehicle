using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using CarClient.DTO;
public class Program
{
    private static ILogger<Program> _logger;
    private static HubConnection _connection;
    private static TelemetryTO _currentTelemetry;
    private static bool _isLocked = true; // The car starts in a locked state.
    private const int UpdateIntervalMs = 2000; // Update every 2 seconds

    // Geospatial transformation objects
    private static ICoordinateTransformation _wgs84ToUtm;
    private static ICoordinateTransformation _utmToWgs84;


    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
            })
            .BuildServiceProvider();

        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var vin = args.Length > 0 ? args[0] : "TESTVIN123456789";
        var hubUrl = configuration["SignalR:HubUrl"];

        if (string.IsNullOrEmpty(hubUrl))
        {
            _logger.LogError("HubUrl is not configured in appsettings.json.");
            return;
        }

        // Initialize Geospatial services
        InitializeCoordinateSystems();

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .AddJsonProtocol(options => // Configure JSON serialization for NetTopologySuite
            {
                options.PayloadSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
            })
            .Build();

        // Initialize car telemetry
        _currentTelemetry = new TelemetryTO
        {
            // Starting point: Near Brandenburg Gate in Berlin (Longitude, Latitude)
            CurrentPosition = new Point(13.3777, 52.5163, 0) { SRID = 4326 },
            CurrentSpeed = 0,
            Heading = 45, // Northeast
            RemainingReach = 350.5
        };

        _logger.LogInformation("Car client initialized for VIN: {VIN}", vin);

        SetupHubEventHandlers();

        if (await ConnectAndRegisterAsync(vin))
        {
            var cts = new CancellationTokenSource();
            _ = Task.Run(() => Drive(cts.Token));

            _logger.LogInformation("Client is running and driving simulation has started. Press Enter to exit.");
            Console.ReadLine();

            cts.Cancel();
            await _connection.DisposeAsync();
        }
    }

    private static void InitializeCoordinateSystems()
    {
        var csFactory = new CoordinateSystemFactory();
        var ctf = new CoordinateTransformationFactory();

        // WGS 84 (EPSG:4326) - Geographic, degrees
        var wgs84 = csFactory.CreateFromWkt("GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]");

        // UTM Zone 33N (EPSG:32633) - Projected, meters. Suitable for Berlin.
        var utm33n = csFactory.CreateFromWkt("PROJCS[\"WGS 84 / UTM zone 33N\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",15],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH],AUTHORITY[\"EPSG\",\"32633\"]]");

        _wgs84ToUtm = ctf.CreateFromCoordinateSystems(wgs84, utm33n);
        _utmToWgs84 = ctf.CreateFromCoordinateSystems(utm33n, wgs84);
    }

    private static void SetupHubEventHandlers()
    {
        _connection.On("RequestLock", (Func<bool>)(() =>
        {
            _logger.LogInformation("Received LOCK request. Stopping car and setting state to locked.");
            _isLocked = true;
            _currentTelemetry.CurrentSpeed = 0;
            var success = true;
            _logger.LogInformation("Lock sequence result: {Success}", success);
            return success;
        }));

        _connection.On("RequestUnlock", (Func<bool>)(() =>
        {
            _logger.LogInformation("Received UNLOCK request. Setting state to unlocked.");
            _isLocked = false;
            var success = true;
            _logger.LogInformation("Unlock sequence result: {Success}", success);
            return success;
        }));
    }

    private static async Task<bool> ConnectAndRegisterAsync(string vin)
    {
        while (true)
        {
            try
            {
                await _connection.StartAsync();
                _logger.LogInformation("Connection established with ID: {ConnectionId}", _connection.ConnectionId);

                var registrationSuccess = await _connection.InvokeAsync<bool>("RegisterCar", vin);
                if (registrationSuccess)
                {
                    _logger.LogInformation("Client registered successfully with VIN: {VIN}", vin);
                    return true;
                }
                else
                {
                    _logger.LogError("Registration failed for VIN '{VIN}'. Shutting down.", vin);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect or register. Retrying in 5 seconds...");
                await Task.Delay(5000);
            }
        }
    }

    private static async Task Drive(CancellationToken token)
    {
        var random = new Random();
        while (!token.IsCancellationRequested)
        {
            // Only update the car's position and speed if it's unlocked.
            if (!_isLocked)
            {
                UpdateSimulation(random);
            }

            try
            {
                // Always send the current state to the server, even if it hasn't changed.
                await _connection.SendAsync("UpdateCarState", _currentTelemetry, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send telemetry update to server.");
            }

            await Task.Delay(UpdateIntervalMs, token);
        }
    }

    private static void UpdateSimulation(Random random)
    {
        // Adjust speed and heading
        if (random.Next(10) > 7) _currentTelemetry.CurrentSpeed += random.Next(-50, 50);
        if (_currentTelemetry.CurrentSpeed < 0) _currentTelemetry.CurrentSpeed = 0;
        if (_currentTelemetry.CurrentSpeed > 180) _currentTelemetry.CurrentSpeed = 180;

        if (random.Next(10) > 5) _currentTelemetry.Heading += random.Next(-10, 11);
        _currentTelemetry.Heading = (_currentTelemetry.Heading + 360) % 360;

        // Calculate distance traveled in meters
        var speedMetersPerSecond = _currentTelemetry.CurrentSpeed * 1000 / 3600;
        var distanceMeters = speedMetersPerSecond * (UpdateIntervalMs / 1000.0);

        // Update remaining reach
        _currentTelemetry.RemainingReach -= distanceMeters / 1000.0; // convert distance to km
        if (_currentTelemetry.RemainingReach <= 0)
        {
            _currentTelemetry.CurrentSpeed = 0;
            _currentTelemetry.RemainingReach = 0;
            return;
        }

        // --- Geospatial Calculation using ProjNet ---

        // 1. Transform current WGS84 point to UTM (meters)
        var currentWgs84Coord = new[] { _currentTelemetry.CurrentPosition.X, _currentTelemetry.CurrentPosition.Y };
        var currentUtmCoord = _wgs84ToUtm.MathTransform.Transform(currentWgs84Coord);

        // 2. Calculate new position in UTM using simple vector math
        var headingRad = _currentTelemetry.Heading * (Math.PI / 180.0);
        var dX = distanceMeters * Math.Sin(headingRad); // Easting
        var dY = distanceMeters * Math.Cos(headingRad); // Northing

        var newUtmCoord = new[] { currentUtmCoord[0] + dX, currentUtmCoord[1] + dY };

        // 3. Transform new UTM point back to WGS84 (degrees)
        var newWgs84Coord = _utmToWgs84.MathTransform.Transform(newUtmCoord);

        // 4. Update the telemetry object, ensuring Z is 0 to prevent JSON errors
        _currentTelemetry.CurrentPosition = new Point(newWgs84Coord[0], newWgs84Coord[1], 0) { SRID = 4326 };

        _logger.LogInformation("Telemetry Update -> Speed: {Speed:F1} km/h, Heading: {Heading:F0}°, Position: ({Lat:F4}, {Lon:F4}), Reach: {Reach:F2} km",
            _currentTelemetry.CurrentSpeed, _currentTelemetry.Heading, _currentTelemetry.CurrentPosition.Y, _currentTelemetry.CurrentPosition.X, _currentTelemetry.RemainingReach);
    }
}
