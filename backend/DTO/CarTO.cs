using NetTopologySuite.Geometries;

namespace SmartContractVehicle.DTO
{
    /***
     * This is a flattend transfer object to make life easier on the front end
     * 
     */
    public class CarTO
    {
        // Information required for the Company
        public required string CompanyName { get; set; }
        public required string CompanyLogoPath { get; set; }

        // Information required for the Model
        public required string ModelName { get; set; }

        // Information required for the Trim
        public required string TrimName { get; set; }
        public required string FueltypeName { get; set; }
        public required string DrivetrainName { get; set; }
        public required string TrimImagePath { get; set; }

        // Information required for the actual Car
        public required Point CurrentPosition { get; set; }
        public double RemainingReach { get; set; }
        public required string Colour { get; set; }
        public required int Seats { get; set; }
        public required double PricePerMinute { get; set; }
    }
}
