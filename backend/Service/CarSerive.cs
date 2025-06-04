using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using SmartContractVehicle.Data;

namespace SmartContractVehicle.Service;

public class CarService
{
    private readonly AppDbContext _context;

    public CarService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Car>> GetAllCarsAsync()
    {
        return await _context.Cars.ToListAsync();
    }
}
