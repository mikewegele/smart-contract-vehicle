using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using NetTopologySuite.Geometries;
using Microsoft.OpenApi.Extensions;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        [HttpGet]
        public ActionResult<IQueryable<CarTO>> Query()
        {
            IList<CarTO> cars = [
                new() {
                    Trim = new VehicleTrimTO
                    {
                        Model = new VehicleModelTO
                        {
                            Producer = new AutomotiveCompanyTO
                            {
                                Name = "BMW",
                            },
                            Name = "iX",
                        },
                        Name = "M60",
                        Fuel = FuelType.Electric.GetDisplayName(),
                        Drivetrain = Drivetrain.AllWheelDrive.GetDisplayName(),
                    },
                    CurrentPosition = new Point(new Coordinate(11.576124, 48.137154)),
                    RemainingReach = 5555.5,
                    Color = "black",
                }
            ];
            
            return Ok(cars);
        }

    }
}
