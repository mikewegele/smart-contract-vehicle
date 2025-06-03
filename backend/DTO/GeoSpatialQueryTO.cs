using NetTopologySuite.Geometries;

namespace SmartContractVehicle.DTO
{
    public class GeoSpatialQueryTO
    {
        public required Point UserLocation { get; set; }

        public required double MaxDistance { get; set; }

        public string[]? AllowedManufactures { get; set; }

        public string[]? AllowedModels { get; set; }

        public string[]? AllowedTrims { get; set; }

        public string[]? AllowedFueltypes { get; set; }

        public string[]? AllowedDrivetrains { get; set; }

        public double? MinRemainingReach { get; set; }

        public int? MinSeats { get; set; }

        public int? MaxSeats { get; set; }

        public double? MinPricePerMinute { get; set; }
        public double? MaxPricePerMinute { get; set; }
    }
}
