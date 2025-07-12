using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace CarClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
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
        if (vins == null || vins.Count == 0)
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

        logger.LogInformation("Shutting down all {VinCount} clients...", vins.Count);
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