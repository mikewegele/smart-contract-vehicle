using Microsoft.EntityFrameworkCore;
using MeinBackend.Models;

namespace MeinBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ExampleModel> Examples { get; set; }
}