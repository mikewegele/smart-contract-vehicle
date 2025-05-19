using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartContractVehicle.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, DataType(DataType.Date)]
        public required DateOnly Birthday { get; set; } 

        [Required]
        public required Address Billing { get; set; }

        [Required]
        public required Address Mailing { get; set; }

        [Required]
        public required string Password { get; set; }

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public required string WalletId { get; set; } 
    }
}
