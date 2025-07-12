namespace SmartContractVehicle.DTO
{
    public class BookingTO
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public string RentorId { get; set; }

        public DateTime StartTimeUTC { get; set; }
        public DateTime? EndTimeUTC { get; set; }
        public bool IsCompleted { get; set; }

        public double? TotalDistance { get; set; }
        public decimal? TotalCost { get; set; }
    }
}