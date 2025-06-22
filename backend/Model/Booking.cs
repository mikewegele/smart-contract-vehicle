using SmartContractVehicle.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartContractVehicle.Model;

public class Booking
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ReservationId { get; set; }  // FK to Reservation
    public Reservation Reservation { get; set; }
    public Guid ReservedCarId { get; private set; }
    public Car Car { get; set;  }
    public Guid CarId { get; private set; }

    public string RentorId { get; private set; }
    public User User { get; set; }

    public DateTime StartTimeUTC { get; private set; } = DateTime.UtcNow;
    public DateTime? EndTimeUTC { get; private set; }

    public bool IsCompleted { get; private set; } = false;

    public double? TotalDistance { get; private set; }
    public decimal? TotalCost { get; private set; }

    public Booking() { }
    public Booking(string rentor)
    {
        RentorId = rentor;
    }
    public void EndRide(double distanceKm, decimal cost)
    {
        IsCompleted = true;
        EndTimeUTC = DateTime.UtcNow;
        TotalDistance = distanceKm;
        TotalCost = cost;
    }
}