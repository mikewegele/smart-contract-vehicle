using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Model
{
   
    public class VehicleTrim
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public virtual VehicleModel Model { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        public virtual FuelType Fuel { get; set; }

        public virtual Drivetrain Drivetrain { get; set; } 

        public required string ImagePath { get; set; }
    }
}
