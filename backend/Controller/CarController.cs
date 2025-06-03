using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using NetTopologySuite.Geometries;
using Microsoft.OpenApi.Extensions;
using AutoMapper;
using SmartContractVehicle.Model;

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


            Car dummy = new()
                    {
                VIN = "WDDTG5CBXFJ045894",
                Colour = "White",
                CurrentPosition = new Point(new Coordinate(48.77854989722567, 9.179689206292418)),
                Owner = _db.Users.First(),
                RemainingReach = 550,
                PricePerMinute = .38,
                SeatNumbers = 4,
                Trim = new VehicleTrim() 
                        {
                    Cars = [],
                    Drivetrain = _db.Drivetrains.Find((int)Drivetrains.AllWheelDrive),
                    Fuel = _db.FuelTypes.Find((int)FuelTypes.Gasoline),
                    ImagePath = "https://www.mercedes-amg.com/media/images/6b934d9c3eae9dd08b822c03115d5085f3a501ef-1352x1014.jpg?auto=format&fit=max&q=75&w=1352",
                    Name = "45 AMG",
                    Model = new VehicleModel()
                            {
                        Name = "GLA",
                        Trims = [],
                        Producer = new AutomotiveCompany()
                        {
                            ImagePath = "https://www.mercedes-amg.com/amg-logo.svg",
                            Models = [],
                            Name = "Mercedes AMG"
                        }
                    }
                }

            };

            CarTO dummyTO = _mapper.Map<CarTO>(dummy);

            IList<CarTO> cars = [
                dummyTO,
                new() {
                     CompanyLogoPath = "",
                     CompanyName = "BMW",
                     DrivetrainName = Drivetrains.AllWheelDrive.GetDisplayName(),
                     FueltypeName = FuelTypes.Electric.GetDisplayName(),
                     ModelName = "iX",
                     TrimImagePath = "",
                     TrimName = "M60",
                    CurrentPosition = new Point(new Coordinate(11.576124, 48.137154)),
                    RemainingReach = 5555.5,
                    Colour = "black",
                    Seats = 5,
                    PricePerMinute = 1,
                }
            ];
            
            return Ok(cars);
        }

    }
}
