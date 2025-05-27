using NetTopologySuite.Geometries;

namespace SmartContractVehicle.DTO
{
    public class CarTO
    {
        public required VehicleTrimTO Trim { get; set; }

        public required Point CurrentPosition { get; set; }

        public double RemainingReach { get; set; }

        public required string Colour { get; set; }
    }
}
