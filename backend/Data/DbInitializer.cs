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

            string[] roles = [RENTER, LESSOR, ADMIN];

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

            string[] brands = ["Tesla", "BMW", "Audi", "Mercedes", "Volkswagen", "Alexander Dennis"];

            string[][] modelNames =
            [
                ["Model S", "Model X"],
                ["X5", "i3"],
                ["A4", "Q7"],
                ["C-Class", "GLA", "Citaro"],
                ["Golf", "Tiguan"],
                ["Enviro500"]
            ];

            string[][][] trimNames =
            [
                [["Long Range", "Plaid"], ["Standard", "Performance"]], // Tesla
                [["xDrive40", "xDrive50"], ["Urban", "Sport"]],         // BMW
                [["Premium", "Sport"], ["Base", "Luxury"]],             // Audi
                [["AMG", "Avantgarde"], ["Base", "Style"], ["Hybrid"]], // Mercedes
                [["Life", "Style"], ["Base", "R-Line"]],                // Volkswagen
                [["Standard", "Double Decker"]]                         // Alexander Dennis
            ];

            var companies = new List<AutomotiveCompany>();
            var cars = new List<Car>();

            for (int b = 0; b < brands.Length; b++)
            {
                var company = new AutomotiveCompany
                {
                    Id = Guid.NewGuid(),
                    Name = brands[b],
                    Models = [],
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
                        Trims = []
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
                            Cars = []
                        };

                        for (int c = 0; c < 10; c++)
                        {
                            var car = new Car(available)
                            {
                                Id = Guid.NewGuid(),
                                VIN = $"VIN{Guid.NewGuid().ToString()[..13]}",
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
