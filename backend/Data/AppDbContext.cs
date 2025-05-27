using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<AutomotiveCompany> AutomotiveCompanys { get; set; }
    public DbSet<VehicleModel> Vehicles { get; set; }
    public DbSet<VehicleTrim> VehicleTrims { get; set; }
    public DbSet<Car> Cars { get; set; }

}