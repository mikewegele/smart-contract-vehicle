using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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
    public ActionResult<IEnumerable<CarTO>> GetAllCars()
    {
         var cars = _db.Cars
             .Include(c => c.Trim)
             .ToList();

         var carTOs = _mapper.Map<List<CarTO>>(cars);
         return Ok(carTOs);
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
        public async Task<ActionResult<List<CarTO>>> GeoSpatialQueryAsync(GeoSpatialQueryTO query, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var allowedGreaterCircleArea = GeometryService.CreateCircleFromPoint(query.UserLocation, query.MaxDistance);

            // Use DB spatial filtering in degrees (approximate filtering)
            var cars = _db.Cars
                .OrderBy(c => c.CurrentPosition.Distance(query.UserLocation))
                .Where(c => allowedGreaterCircleArea.Covers(c.CurrentPosition));

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


            return Ok(data);
        }

        [HttpGet]
        public ActionResult<IQueryable<DrivetrainTO>> GetDrivetrains()
        {
            var drivetrains = _db.Drivetrains;
            IQueryable res = drivetrains.Select(d => _mapper.Map<DrivetrainTO>(d));
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<List<FuelTypeTO>>> GetFueltypes()
        {
            var result = await _db.FuelTypes
                                  .Select(d => new FuelTypeTO
                                  {
                                      Name = d.Name
                                  })
                                  .ToListAsync();

            return Ok(result);
        }

        [HttpGet]
        public ActionResult<IQueryable<CarStatusTO>> GetCarStatuses()
        {
            var carstatuses = _db.CarStatuses.Select(cs => _mapper.Map<CarStatusTO>(cs));
            return Ok(carstatuses);
        }

        [HttpGet]
        public ActionResult<CarStatusTO> GetStatus(Guid carId)
        {
            var car = _db.Cars.Find(carId);
            if (car == null)
                return NotFound("Car not found.");

            return Ok(_mapper.Map<CarStatusTO>(car.Status));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutomotiveCompanyTO>>> GetAutomotiveCompanies(CancellationToken ct)
        {
            var companies = await _db.AutomotiveCompanies.ToArrayAsync(ct);

            IEnumerable<AutomotiveCompanyTO> res = companies.Select(_mapper.Map<AutomotiveCompanyTO>);

            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleModelTO>>> GetModels(string? ManufactureName, CancellationToken ct)
        {
            IQueryable<VehicleModel> vehiclemodels = _db.VehicleModels;

            if (!string.IsNullOrEmpty(ManufactureName) && !string.IsNullOrWhiteSpace(ManufactureName))
            {
                vehiclemodels = vehiclemodels.Where(vm => EF.Functions.ILike(vm.Producer.Name.Trim(), ManufactureName.Trim()));
            }

            IEnumerable<VehicleModelTO> res = (await vehiclemodels.ToArrayAsync(ct)).Select(_mapper.Map<VehicleModelTO>);

            return Ok(res);

        }

        [HttpGet]
        public async Task<IActionResult> GetTrims(string? ModelName, CancellationToken ct)
        {
            IQueryable<VehicleTrim> vehicletrims = _db.VehicleTrims;

            if (!string.IsNullOrEmpty(ModelName) && !string.IsNullOrWhiteSpace(ModelName))
            {
                vehicletrims = vehicletrims.Where(vt => EF.Functions.ILike(vt.Model.Name.Trim(), ModelName.Trim()));
            }


            IEnumerable<VehicleTrimTO> res = (await vehicletrims.ToArrayAsync(ct)).Select(_mapper.Map<VehicleTrimTO>);

            return Ok(res);
        }
    }
}
