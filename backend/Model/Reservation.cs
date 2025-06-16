namespace SmartContractVehicle.Model;

public class Reservation
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string RentorId { get; private set; }
    public Guid ReservedCarId { get; private set; }

    public double Price { get; private set; }
    public DateTime ReservationTimeUTC { get; private set; } = DateTime.UtcNow;

    private string? _blockchainTransactionId;
    public string? BlockchainTransactionId
    {
        get => _blockchainTransactionId;
        set
        {
            if (_blockchainTransactionId is null)
                _blockchainTransactionId = value;
            else
                throw new InvalidOperationException("BlockchainTransactionId can only be set once.");
        }
    }

    // Parameterless constructor for EF
    private Reservation() { }

    // Constructor with logic
    public Reservation(Guid reservedCarId, string rentorId, double carPricePerMinute)
    {
        ReservedCarId = reservedCarId;
        RentorId = rentorId;
        Price = carPricePerMinute / 2 * 15;
    }
}
