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

        public bool IsAdmin { get; set; } = false;
        
        public bool IsRenter { get; set; } = true;

        public bool IsLessor { get; set; } = false;

        public virtual ICollection<Car>? Cars { get; set; }

/*
        [Required, DataType(DataType.Date)]
        public required DateOnly Birthday { get; set; }

        [Required]
        public required Address Billing { get; set; }

        [Required]
        public required Address Mailing { get; set; }

        [Required]
        public required string WalletId { get; set; }
        */
    }
}
