using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.DTO
{
    public class GeoSpatialQueryTO
    {
        public required Point UserLocation { get; set; }

        [Range(0, 10_000_000_000)] // Distance is probably calculated in meters
        public required double MaxDistance { get; set; }

        public string[]? AllowedManufactures { get; set; }

        public string[]? AllowedModels { get; set; }

        public string[]? AllowedTrims { get; set; }

        public string[]? AllowedFueltypes { get; set; }

        public string[]? AllowedDrivetrains { get; set; }

        [Range(0, 1_000)] // This reach is given in km
        public double? MinRemainingReach { get; set; }

        [Range(1, 150)] //  Means you can have a motorbike or a large bus
        public int? MinSeats { get; set; }
        [Range(1, 150)] //  Means you can have a motorbike or a large bus
        public int? MaxSeats { get; set; }

        [Range(0, 5)] // If you pay more than 5€ per Minute we have a Problem
        public double? MinPricePerMinute { get; set; }

        [Range(0, 5)] // If you pay more than 5€ per Minute we have a Problem
        public double? MaxPricePerMinute { get; set; }
    }
}
