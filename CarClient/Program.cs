using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

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
        // HINWEIS: Dies erfordert die folgenden NuGet-Pakete:
        // - Microsoft.Extensions.Logging
        // - Microsoft.Extensions.Logging.Configuration
        // - Microsoft.Extensions.Logging.Console
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

        // Definieren, was passiert, wenn der Server einen "LockCommand" sendet
        connection.On("LockCommand", () =>
        {
            logger.LogInformation("Received LOCK command. Executing lock sequence...");
            // TODO: Hier die eigentliche Logik zum Verriegeln des Autos hinzufügen
        });

        // Definieren, was passiert, wenn der Server einen "UnlockCommand" sendet
        connection.On("UnlockCommand", () =>
        {
            logger.LogInformation("Received UNLOCK command. Executing unlock sequence...");
            // TODO: Hier die eigentliche Logik zum Entriegeln des Autos hinzufügen
        });

        // Schleife zur Handhabung der Verbindung und Registrierung
        while (true)
        {
            try
            {
                await connection.StartAsync();
                logger.LogInformation("Connection established successfully with Connection ID: {ConnectionId}", connection.ConnectionId);

                // Das Auto registrieren und das Ergebnis überprüfen
                var registrationSuccess = await connection.InvokeAsync<bool>("RegisterCar", vin);

                if (registrationSuccess)
                {
                    logger.LogInformation("Client registered successfully with VIN: {VIN}", vin);
                    break; // Verbindungsschleife beenden und den Client weiterlaufen lassen
                }
                else
                {
                    logger.LogError("Registration failed. The VIN '{VIN}' is invalid or not found in the database. Shutting down.", vin);
                    return; // Anwendung beenden
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
