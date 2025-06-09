using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using NetTopologySuite.Geometries;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using SmartContractVehicle.Service;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarController(AppDbContext db, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public ActionResult<IQueryable<CarTO>> GetAllCars()
        {

            var cars = _db.Cars
                .Include(c => c.Trim)
                .ThenInclude(t => t.Fuel)
                .Include(c => c.Trim)
                .ThenInclude(t => t.Drivetrain)
                .Include(c => c.Trim)
                .ThenInclude(t => t.Model)
                .ThenInclude(m => m.Producer)
                .Include(c => c.Owner)
                .Select(c => _mapper.Map<CarTO>(c));
            return Ok(cars);
        }

        [HttpGet]
        public ActionResult<IQueryable<CarTO>> GetDummyData()
        {

            IList<CarTO> cars = [
                new() {
                     CarId = new Guid(),
                     CompanyLogoPath = "",
                     CompanyName = "BMW",
                     DrivetrainName = Drivetrains.AllWheelDrive.GetDisplayName(),
                     FueltypeName = FuelTypes.Electric.GetDisplayName(),
                     ModelName = "iX",
                     TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/images/bmw-ix.png",
                     TrimName = "M60",
                    CurrentPosition = new Point(new Coordinate(11.576124, 48.137154)),
                    RemainingReach = 5555.5,
                    Colour = "black",
                    Seats = 5,
                    PricePerMinute = 1,
                },
                new() {
                    CarId = new Guid(),
                    CompanyLogoPath = "",
                    CompanyName = "BMW",
                    DrivetrainName = Drivetrains.RearWheelDrive.GetDisplayName(),
                    Colour = "Blue",
                    CurrentPosition = new Point(new Coordinate(13.39, 52.515)),
                    FueltypeName = FuelTypes.Electric.GetDisplayName(),
                    ModelName = "i3",
                    PricePerMinute = .35,
                    Seats = 4,
                    TrimName = "I01",
                    RemainingReach = 250,
                    TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/images/bmw-i3.png",
                },
                new() {
                    CarId = new Guid(),
                    CompanyLogoPath = "",
                    CompanyName = "Fiat",
                    DrivetrainName = Drivetrains.FrontWheelDrive.GetDisplayName(),
                    Colour = "White",
                    CurrentPosition = new Point(new Coordinate (13.405, 52.52)),
                    FueltypeName = FuelTypes.Electric.GetDisplayName(),
                    ModelName = "500e Limousine",
                    Seats = 4, 
                    PricePerMinute = .25,
                    RemainingReach = 230,
                    TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/images/fiat-500e-limousine.png",
                    TrimName = "La Prima 87 kW (118PS)"
                },
                new() {
                    CarId = new Guid(),
                    CompanyLogoPath = "",
                    CompanyName = "Alexander Dennis",
                    DrivetrainName = Drivetrains.RearWheelDrive.GetDisplayName(),
                    Colour = "Yellow",
                    CurrentPosition = new Point(new Coordinate(13.396952, 52.421153)),
                    FueltypeName = FuelTypes.Diesel.GetDisplayName(),
                    ModelName = "ADL Enviro500",
                    Seats = 80,
                    PricePerMinute = 2.0,
                    RemainingReach = 600,
                    TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/images/alexander-dennis-enviro500.png",
                    TrimName = "Alexander Dennis Enviro500"
                },
            ];
            
            return Ok(cars);
        }


        /** 
         * JSON Call Beispiel:
           {
              "userLocation": {
                  "type": "Point",
                  "coordinates": [
                   13.25, 52.5 
                  ]
                },
                "maxDistance" : 5000,
                "minRemainingReach" : 200,
                "minSeats" : 4,
                "maxSeats" : 5,
                "minPricePerMinute" : 0.3,
                "maxPricePerMinute" : 0.7,
                "allowedTrims" : ["avantgarde", "life" ],
                "allowedModels" : ["E-Class", "C-Class"],
                "allowedManufactures" : ["Tesla", "Mercedes" ],
                "allowedDrivetrains" : [ "All-Wheel Drive (AWD)", "Four-Wheel Drive (4WD)"],
                "allowedFueltypes" : ["Ethanol", "Diesel" ]
            }
        */

        [HttpPost]
        public async Task<IActionResult> GeoSpatialQueryAsync(GeoSpatialQueryTO query, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var allowedGreaterCircleArea = GeometryService.CreateCircleFromPoint(query.UserLocation, query.MaxDistance);

            // Use DB spatial filtering in degrees (approximate filtering)
            var cars = _db.Cars
                .OrderBy(c => c.CurrentPosition.Distance(query.UserLocation))
                .Where(c => allowedGreaterCircleArea.Covers(c.CurrentPosition));
            // Rough conversion from meters to degrees

            if (query.AllowedManufactures is not null && query.AllowedManufactures.Length > 0)
            {
                var allowedManufactures = query.AllowedManufactures.Select(m => m.Normalize()).Distinct();
                cars = cars.Where(c => allowedManufactures.Any(ap => EF.Functions.ILike(c.Trim.Model.Producer.Name, ap)));
            }

            if (query.AllowedModels is not null && query.AllowedModels.Length > 0)
            {
                var AllowedModels = query.AllowedModels.Select(m => m.Normalize()).Distinct();
                cars = cars.Where(c => AllowedModels.Any(am => EF.Functions.ILike(c.Trim.Model.Name, am)));
            }

            if (query.AllowedTrims is not null && query.AllowedTrims.Length > 0)
            {
                var AllowedTrims = query.AllowedTrims.Select(m => m.Normalize()).Distinct();
                cars = cars.Where(c => AllowedTrims.Any(at => EF.Functions.ILike(c.Trim.Name, at)));
            }

            if (query.AllowedFueltypes is not null && query.AllowedFueltypes.Length > 0)
            {
                var AllowedFueltypes = query.AllowedFueltypes.Select(f => f.GetEnumFromDisplayName<FuelTypes>()).Aggregate((f1, f2) => f1 | f2);
                cars = cars.Where(c => AllowedFueltypes.HasFlag((FuelTypes)c.Trim.Fuel.Id));
            }

            if (query.AllowedDrivetrains is not null && query.AllowedDrivetrains.Length > 0)
            {
                var AllowedDrivetrains = query.AllowedDrivetrains.Select(f => f.GetEnumFromDisplayName<Drivetrains>());
                cars = cars.Where(c => AllowedDrivetrains.Contains((Drivetrains)c.Trim.Drivetrain.Id));
            }

            if (query.MinRemainingReach is not null)
            {
                cars = cars.Where(c => c.RemainingReach >= query.MinRemainingReach);
            }

            if (query.MinSeats is not null)
            {
                cars = cars.Where(c => c.SeatNumbers >= query.MinSeats);
            }

            if (query.MaxSeats is not null)
            {
                cars = cars.Where(c => c.SeatNumbers <= query.MaxSeats);
            }

            if (query.MinPricePerMinute is not null)
            {
                cars = cars.Where(c => c.PricePerMinute >= query.MinPricePerMinute);
            }

            if (query.MaxPricePerMinute is not null)
            {
                cars = cars.Where(c => c.PricePerMinute <= query.MaxPricePerMinute);
            }

            var result = await cars.ToArrayAsync(token);

            var data = result.Select(c => _mapper.Map<CarTO>(c)).ToList();


            return Ok(new { count = data.Count, data });
        }



        [HttpGet]
        public IActionResult GetDrivetrains(bool WithId)
        {
            var drivetrains = _db.Drivetrains;

            IQueryable res = (WithId) ? drivetrains.Select(d => new { d.Name, d.Id }) : drivetrains.Select(d => new { d.Name });

            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetFueltypes(bool WithId)
        {
            var fueltypes = _db.FuelTypes;

            IQueryable res = (WithId) ? fueltypes.Select(d => new { d.Name, d.Id }) : fueltypes.Select(d => new { d.Name });

            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetAutomotiveCompanies(bool WithId)
        {
            var companies = _db.AutomotiveCompanies;

            IQueryable res = (WithId) ? companies.Select(d => new { d.Name, d.Id }) : companies.Select(d => new { d.Name });

            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetModels(string? ManufactureName, bool WithId)
        {
            IQueryable<VehicleModel> vehiclemodels = _db.VehicleModels;
            
            if (!string.IsNullOrEmpty(ManufactureName) && !string.IsNullOrWhiteSpace(ManufactureName))
            {
                vehiclemodels = vehiclemodels.Where(vm => EF.Functions.ILike(vm.Producer.Name.Trim(), ManufactureName.Trim()));
            }
                

            IQueryable res = (WithId) ? vehiclemodels.Select(d => new { d.Name, d.Id }) : vehiclemodels.Select(d => new { d.Name });

            return Ok(res);

        }

        [HttpGet]
        public IActionResult GetTrims(string? ModelName, bool WithId)
        {
            IQueryable<VehicleTrim> vehicletrims = _db.VehicleTrims;

            if (!string.IsNullOrEmpty(ModelName) && !string.IsNullOrWhiteSpace(ModelName))
            {
                vehicletrims = vehicletrims.Where(vt => EF.Functions.ILike(vt.Model.Name.Trim(), ModelName.Trim()));
            }
                

            IQueryable res = (WithId) ?  vehicletrims.Select(d =>  new { d.Name, d.Id })  : vehicletrims.Select(d => new { d.Name });

            return Ok(res);
        }

    }
}
