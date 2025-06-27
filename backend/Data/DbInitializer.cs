using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Data
{
    public static class DbInitializer
    {
        public const string RENTER = "renter";
        public const string LESSOR = "lessor";
        public const string ADMIN = "admin";

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { RENTER, LESSOR, ADMIN };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static (List<AutomotiveCompany> Companies, List<Car> Cars) SeedCars(User user, FuelType[] fuels, Drivetrain[] drivetrains, CarStatus available) 
        {
            var random = new Random();
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            string[] brands = new string[] { "Tesla", "BMW", "Audi", "Mercedes", "Volkswagen", "Alexander Dennis" };

            string[][] modelNames = new string[][]
            {
                new string[] { "Model S", "Model X" },
                new string[] { "X5", "i3" },
                new string[] { "A4", "Q7" },
                new string[] { "C-Class", "GLA", "Citaro" },
                new string[] { "Golf", "Tiguan" },
                new string[] { "Enviro500" }
            };

            string[][][] trimNames = new string[][][]
            {
                new string[][] { new string[] { "Long Range", "Plaid" }, new string[] { "Standard", "Performance" } }, // Tesla
                new string[][] { new string[] { "xDrive40", "xDrive50" }, new string[] { "Urban", "Sport" } },          // BMW
                new string[][] { new string[] { "Premium", "Sport" }, new string[] { "Base", "Luxury" } },              // Audi
                new string[][] { new string[] { "AMG", "Avantgarde" }, new string[] { "Base", "Style" }, new string[] { "Hybrid" } }, // Mercedes
                new string[][] { new string[] { "Life", "Style" }, new string[] { "Base", "R-Line" } },                // Volkswagen
                new string[][] { new string[] { "Standard", "Double Decker" } }                                        // Alexander Dennis
            };

            var companies = new List<AutomotiveCompany>();
            var cars = new List<Car>();

            for (int b = 0; b < brands.Length; b++)
            {
                var company = new AutomotiveCompany
                {
                    Id = Guid.NewGuid(),
                    Name = brands[b],
                    Models = new List<VehicleModel>(),
                    ImagePath = $"https://mikewegele.github.io/smart-contract-vehicle/images/{brands[b].ToLower().Replace(" ", "")}.png"
                };

                for (int m = 0; m < modelNames[b].Length; m++)
                {
                    // Sicherstellen, dass genug Trim-Infos vorhanden sind
                    if (m >= trimNames[b].Length)
                        continue;

                    var model = new VehicleModel
                    {
                        Id = Guid.NewGuid(),
                        Name = modelNames[b][m],
                        Producer = company,
                        Trims = new List<VehicleTrim>()
                    };

                    for (int t = 0; t < trimNames[b][m].Length; t++)
                    {
                        var trim = new VehicleTrim
                        {
                            Id = Guid.NewGuid(),
                            Name = trimNames[b][m][t],
                            Model = model,
                            Fuel = fuels[random.Next(fuels.Length)],
                            Drivetrain = drivetrains[random.Next(drivetrains.Length)],
                            ImagePath = $"https://mikewegele.github.io/smart-contract-vehicle/images/{brands[b].ToLower().Replace(" ", "")}_{model.Name.ToLower().Replace(" ", "")}.png",
                            Cars = new List<Car>()
                        };

                        for (int c = 0; c < 10; c++)
                        {
                            var car = new Car(available)
                            {
                                Id = Guid.NewGuid(),
                                VIN = $"VIN{Guid.NewGuid().ToString().Substring(0, 13)}",
                                Owner = user,
                                Trim = trim,
                                CurrentPosition = geometryFactory.CreatePoint(new Coordinate(
                                    13.2 + random.NextDouble() * 0.4,
                                    52.3 + random.NextDouble() * 0.3
                                )),
                                RemainingReach = Math.Round(random.NextDouble() * 400 + 100, 2),
                                Colour = new[] { "Red", "Blue", "Black", "White", "Grey", "Orange", "Yellow", "Pearl", "Green" }[random.Next(9)],
                                SeatNumbers = 4 + random.Next(3),
                                PricePerMinute = Math.Round(random.NextDouble() * 0.5 + 0.1, 2),
                            };
                            trim.Cars.Add(car);
                            cars.Add(car);
                        }

                        model.Trims.Add(trim);
                    }

                    company.Models.Add(model);
                }

                companies.Add(company);
            }

            return (companies, cars);
        }

    }
}
