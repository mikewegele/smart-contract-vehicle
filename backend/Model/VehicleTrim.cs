using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Model
{
   
    public class VehicleTrim
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public required VehicleModel Model { get; set; }

        public required List<Car> Cars { get; set; }
        // public required string ImagePath { get; set; }
    }
}
