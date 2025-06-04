using NetTopologySuite.Geometries;

namespace SmartContractVehicle.DTO
{
    public class CarTO
    {
        public required string Id { get; set; }

        public required string VIN { get; set; }

        public required User Owner { get; set; }

        public required VehicleTrim Trim { get; set; }

        [Column(TypeName = "geography")]
        public required Point CurrentPosition { get; set; }

        public double RemainingReach { get; set; }

        public required string Colour { get; set; }
    }
}
