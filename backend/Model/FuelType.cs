using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Model
{
    public class FuelType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

    }
}
