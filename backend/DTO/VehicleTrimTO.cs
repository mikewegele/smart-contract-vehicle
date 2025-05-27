namespace SmartContractVehicle.DTO
{
    public class VehicleTrimTO
    {
        public required VehicleModelTO Model { get; set; }

        public required string Name { get; set; }

        public required string Fuel { get; set; }

        public required string Drivetrain { get; set; }

        public string? ImagePath { get; set; }

    }
}
