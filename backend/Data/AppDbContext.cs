using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Models;

namespace SmartContractVehicle.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ExampleModel> Examples { get; set; }
}