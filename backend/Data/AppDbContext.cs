using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<AutomotiveCompany> AutomotiveCompanies { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<VehicleTrim> VehicleTrims { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<FuelType> FuelTypes { get; set; }
    public DbSet<Drivetrain> Drivetrains { get; set; }
    public DbSet<CarStatus> CarStatuses { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        FuelTypeSeeder.Seed(modelBuilder);
        DrivetrainSeeder.Seed(modelBuilder);
        CarStatusSeeder.Seed(modelBuilder);

        modelBuilder.Entity<Reservation>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.RentorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne<Car>()
            .WithMany()
            .HasForeignKey(r => r.ReservedCarId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}