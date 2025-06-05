using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using NetTopologySuite.Geometries;
using Microsoft.OpenApi.Extensions;
using AutoMapper;
using SmartContractVehicle.Model;
using System;
using Microsoft.EntityFrameworkCore;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarController(AppDbContext db, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;

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
                     TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/iamges/bmw-ix",
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
                    TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/iamges/bmw-i3",
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
                    TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/iamges/fiat-500e-limousine.png",
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
                    TrimImagePath = "https://mikewegele.github.io/smart-contract-vehicle/iamges/alexander-dennis-enviro500.png",
                    TrimName = "Alexander Dennis Enviro500"
                },
            ];
            
            return Ok(cars);
        }


        [HttpGet]
        public IActionResult GeoSpatialQuery(GeoSpatialQueryTO query)
        {
            if(!ModelState.IsValid)
            {  return BadRequest(ModelState); }

            var cars = _db.Cars
                .OrderBy(c => c.CurrentPosition.Distance(query.UserLocation))
                .Where(c => c.CurrentPosition.IsWithinDistance(query.UserLocation, query.MaxDistance));

            if (query.AllowedManufactures is not null)
            {
                var allowedManufactures = query.AllowedManufactures.Select(m => m.Normalize()).Distinct();
                cars = cars.Where(c => allowedManufactures.Contains(c.Trim.Model.Producer.Name.Normalize()));
            }

            if (query.AllowedModels is not null)
            {
                var AllowedModels = query.AllowedModels.Select(m => m.Normalize()).Distinct();
                cars = cars.Where(c => AllowedModels.Contains(c.Trim.Model.Name.Normalize()));
            }

            if (query.AllowedTrims is not null)
            {
                var AllowedTrims = query.AllowedTrims.Select(m => m.Normalize()).Distinct();
                cars = cars.Where(c => AllowedTrims.Contains(c.Trim.Name.Normalize()));
            }

            if (query.AllowedFueltypes is not null)
            {
                var AllowedFueltypes = query.AllowedFueltypes.Select(f => f.GetEnumFromDisplayName<FuelTypes>()).Aggregate((f1, f2) => f1 | f2);
                cars = cars.Where(c => AllowedFueltypes.HasFlag((FuelTypes)c.Trim.Fuel.Id) );
            }

            if (query.AllowedDrivetrains is not null)
            { 
                var AllowedDrivetrains = query.AllowedDrivetrains.Select(f => f.GetEnumFromDisplayName<Drivetrains>());
                cars = cars.Where(c => AllowedDrivetrains.Contains((Drivetrains)c.Trim.Drivetrain.Id));
            }

            if (query.MinRemainingReach is not null)
            {
                cars = cars.Where(c => c.RemainingReach >=  query.MinRemainingReach);
            }

            if (query.MinSeats is not null)
            {
                cars = cars.Where(c => c.SeatNumbers >=  query.MinSeats);
            }

            if (query.MaxSeats is not null)
            {
                cars = cars.Where(c => c.SeatNumbers <=  query.MaxSeats);
            }

            if (query.MinPricePerMinute is not null)
            {
                cars = cars.Where(c => c.PricePerMinute >= query.MinPricePerMinute);
            }

            if (query.MaxPricePerMinute is not null)
            {
                cars = cars.Where(c => c.PricePerMinute <= query.MaxPricePerMinute);
            }


            return Ok(cars.Select(c => _mapper.Map<CarTO>(c)));
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
