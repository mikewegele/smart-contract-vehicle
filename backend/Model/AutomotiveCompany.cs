using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Model
{
    public class AutomotiveCompany
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        public virtual ICollection<VehicleModel> Models { get; set; }

        public string ImagePath { get; set; } // This should be a path to the file with the Logo of this Company (optional)
    }
}
