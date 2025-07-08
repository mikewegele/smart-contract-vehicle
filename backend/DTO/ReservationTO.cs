using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.DTO
{
    public class ReservationTO
    {
        public Guid Id { get; set; }

        [Required]
        public required string RentorId { get; set; }

        [Required]
        public required Guid ReservedCarId { get; set; }
        public double Price { get; set; }

        public DateTime? ReservationTimeUTC { get; set; }
        public DateTime? BlockageTimeUTC { get; set; }

        public string BlockchainTransactionId { get; set; } = string.Empty;

        public bool ReservationCompleted { get; set; }
        public bool ReservationCancelled { get; set; }

    }

}
