using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartContractVehicle.Models
{
    public class Address
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(10)]
        public string? ExtraInfo { get; set; } // e.g. Apartment number 

        [StringLength(50)]
        public string? Floor { get; set; }

        [Required]
        public int HouseNumber { get; set; }

        [Required, StringLength(50)]
        public required string Street { get; set; }

        [Required, StringLength(50)]
        public required string City { get; set; }

        [Required, StringLength(15)]
        public required string PostalCode { get; set; }

        [Required, StringLength(110)]
        public required string Country { get; set; }

    }
}
