using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace SmartContractVehicle.Model
{
    [Index(nameof(VIN), IsUnique = true)]
    public class Car
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string VIN { get; set; }

        public required User Owner { get; set; }

        public required VehicleTrim Trim { get; set; }

        [Column(TypeName = "geography")]
        public required Point CurrentPosition { get; set; }

        public double RemainingReach { get; set; }

        public required string Colour { get; set; }
    }
}

