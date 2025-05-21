using Microsoft.EntityFrameworkCore;
using User.Model;

namespace SmartContractVehicle.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User.Model.User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
}