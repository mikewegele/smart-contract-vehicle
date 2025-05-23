using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartContractVehicle.Model
{
    public class User : IdentityUser
    {
        [Required]
        public required string Name { get; set; }

/*
        [Required, DataType(DataType.Date)]
        public required DateOnly Birthday { get; set; }

        [Required]
        public required Address Billing { get; set; }

        [Required]
        public required Address Mailing { get; set; }

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public required string WalletId { get; set; }
        */
    }
}
