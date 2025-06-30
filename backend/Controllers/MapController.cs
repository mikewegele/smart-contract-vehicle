// MapController.cs
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Controllers;

public class MapController : Controller
{
    private readonly TelemetryService _telemetryService;

    public MapController(TelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    public IActionResult Index()
    {
        // We pass the initial list of cars to the view to render the sidebar.
        // The map itself will be populated by a SignalR call from the client.
        var allCars = _telemetryService.GetAllCarStates();
        return View(allCars);
    }
}
