using NetTopologySuite.Geometries;

namespace SmartContractVehicle.DTO
{
    public class CarTO
    {
        public required string Id { get; set; }

        public required string VIN { get; set; }

        public required User Owner { get; set; }

        [Column(TypeName = "geography")]
        public required Point CurrentPosition { get; set; }

        public double RemainingReach { get; set; }

        public required string Color { get; set; }

        public required string Manufacturer { get; set; }

        public required string Model { get; set; }

        public int Year { get; set; }
    }
}
