using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Model
{
    
    public class VehicleModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public required AutomotiveCompany Producer { get; set; }

        public required ICollection<VehicleTrim> Trims { get; set; }

    }
}
