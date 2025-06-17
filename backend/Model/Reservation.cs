using SmartContractVehicle.Data;

namespace SmartContractVehicle.Model;

public class Reservation
{
    public static readonly TimeSpan _reservationTime = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan _blockageTime = TimeSpan.FromMinutes(4);

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string RentorId { get; private set; }
    public Guid ReservedCarId { get; private set; }

    public double Price { get; private set; }

    public bool ReservationActive => 
        !ReservationCompleted &&
        !ReservationCancelled && 
        ReservationTimeUTC != null &&
        DateTime.UtcNow.Subtract(ReservationTimeUTC.Value) < _reservationTime;

    public DateTime? ReservationTimeUTC { get; private set; }
    
    public bool BlockageActive => BlockageTimeUTC != null && DateTime.UtcNow.Subtract(BlockageTimeUTC.Value) < _blockageTime;
    public DateTime? BlockageTimeUTC { get; private set; }


    private string _blockchainTransactionId = string.Empty;
    public string BlockchainTransactionId
    {
        get => _blockchainTransactionId;
        set
        {
            if (_blockchainTransactionId == string.Empty)
                _blockchainTransactionId = value;
            else
                throw new InvalidOperationException("BlockchainTransactionId can only be set once.");
        }
    }

    public bool ReservationCompleted { get; private set; } = false;
    public bool ReservationCancelled { get; private set; } = false;

    public Reservation() { }
    
    public Reservation(string rentor) 
    {
        RentorId = rentor;
    }

    public Reservation BlockCar(AppDbContext db, Car car)
    {
        car.SetStatus(db.CarStatuses.Find((int)CarStatuses.Pending), this);

        ReservedCarId = car.Id;
        Price = car.PricePerMinute / 2 * _reservationTime.TotalMinutes;
        BlockageTimeUTC = DateTime.UtcNow;

        return this;
    }

    public Reservation ReserveCar(AppDbContext db) 
    {
        var car = db.Cars.Find(ReservedCarId);
        if (car is null) throw new NullReferenceException($"{nameof(car)}, The car wasn't found in the Database.");

        car.SetStatus(db.CarStatuses.Find((int)CarStatuses.Reserved), this);

        ReservationTimeUTC = DateTime.UtcNow;

        return this;
    }

    public Reservation CancleReservation(AppDbContext db)
    {
        var car = db.Cars.Find(ReservedCarId);
        if (car is null) throw new NullReferenceException($"{nameof(car)}, The car wasn't found in the Database.");

        car.SetStatus(db.CarStatuses.Find((int)CarStatuses.Available), this);

        ReservationCancelled = true;
        ReservationCompleted = false;

        return this;
    }

    public Reservation CompleteReservation(AppDbContext db)
    {
        var car = db.Cars.Find(ReservedCarId);
        if (car is null) throw new NullReferenceException($"{nameof(car)}, The car wasn't found in the Database.");

        car.SetStatus(db.CarStatuses.Find((int)CarStatuses.InTransit), this);

        ReservationCancelled = false;
        ReservationCompleted = true;

        return this;
    }

}
