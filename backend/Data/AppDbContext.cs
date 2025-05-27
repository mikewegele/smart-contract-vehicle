using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<AutomotiveCompany> AutomotiveCompanys { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<VehicleTrim> VehicleTrims { get; set; }
    public DbSet<Car> Cars { get; set; }

    public DbSet<Model.FuelType> FuelTypes { get; set; }

    public DbSet<Model.Drivetrain> Drivetrains { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        FuelTypeSeeder.Seed(modelBuilder);
        DrivetrainSeeder.Seed(modelBuilder);
    }


}