namespace SmartContractVehicle.DTO
{
    public class VehicleTrimTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public Guid Model { get; set; }

        public Guid[] Cars { get; set; }

        public int FuelId { get; set; }

        public int DrivetrainId { get; set; }

        public required string ImagePath { get; set; }
    }
}
