using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Konfiguration aus appsettings.json einrichten
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Dependency Injection einrichten
        var services = new ServiceCollection();

        // Logging-Dienste hinzufügen.
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
        });

        // Service Provider erstellen
        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // VIN aus Kommandozeilenargumenten oder einen Standardwert für Tests verwenden.
        var vin = args.Length > 0 ? args[0] : "TESTVIN123456789";

        // Hub-URL aus der Konfigurationsdatei lesen
        var hubUrl = configuration["SignalR:HubUrl"];

        if (string.IsNullOrEmpty(hubUrl))
        {
            logger.LogError("HubUrl ist in appsettings.json nicht konfiguriert. Bitte überprüfen Sie die Konfiguration.");
            return;
        }

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        logger.LogInformation("Car client initialized for VIN: {VIN}", vin);
        logger.LogInformation("Connecting to hub at {HubUrl}...", hubUrl);

        // Definieren, was passiert, wenn der Server einen Sperrbefehl anfordert.
        // Der Handler wird explizit als Func<bool> gecastet, um sicherzustellen, dass ein Ergebnis zurückgegeben wird.
        connection.On("RequestLock", (Func<bool>)(() =>
        {
            logger.LogInformation("Received LOCK request. Executing lock sequence...");
            // TODO: Hier die eigentliche Logik zum Verriegeln des Autos hinzufügen.
            // Geben Sie 'true' zurück, wenn erfolgreich, andernfalls 'false'.
            var success = true; // Simulieren eines erfolgreichen Vorgangs
            logger.LogInformation("Lock sequence result: {Success}", success);
            return success;
        }));

        // Definieren, was passiert, wenn der Server einen Entsperrbefehl anfordert.
        connection.On("RequestUnlock", (Func<bool>)(() =>
        {
            logger.LogInformation("Received UNLOCK request. Executing unlock sequence...");
            // TODO: Hier die eigentliche Logik zum Entriegeln des Autos hinzufügen.
            var success = true; // Simulieren eines erfolgreichen Vorgangs
            logger.LogInformation("Unlock sequence result: {Success}", success);
            return success;
        }));

        // Schleife zur Handhabung der Verbindung und Registrierung
        while (true)
        {
            try
            {
                await connection.StartAsync();
                logger.LogInformation("Connection established successfully with Connection ID: {ConnectionId}", connection.ConnectionId);

                var registrationSuccess = await connection.InvokeAsync<bool>("RegisterCar", vin);

                if (registrationSuccess)
                {
                    logger.LogInformation("Client registered successfully with VIN: {VIN}", vin);
                    break;
                }
                else
                {
                    logger.LogError("Registration failed. The VIN '{VIN}' is invalid or not found in the database. Shutting down.", vin);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to connect or register. Retrying in 5 seconds...");
                await Task.Delay(5000);
            }
        }

        logger.LogInformation("Client is running. Press Enter to exit.");
        Console.ReadLine();

        await connection.DisposeAsync();
    }
}
