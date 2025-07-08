using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.DTO
{
    public class AutomotiveCompanyTO
    {
        public Guid Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        public required Guid[] Models { get; set; }
        public string? ImagePath { get; set; }
    }
}
