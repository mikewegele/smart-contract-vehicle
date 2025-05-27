using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Model
{
    
    public class VehicleModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required AutomotiveCompany Producer { get; set; }

        public required List<VehicleTrim> Trims { get; set; }

    }
}
