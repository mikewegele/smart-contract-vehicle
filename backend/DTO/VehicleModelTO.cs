namespace SmartContractVehicle.DTO
{
    public class VehicleModelTO
    {
        public required Guid Id { get; set; }
        public required string Name {  get; set; }
        public required Guid ProducerId { get; set; }
        public required Guid[] Trims { get; set; }
    }
}
