using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SmartContractVehicle.Data;

namespace SmartContractVehicle.Model
{
    [Index(nameof(VIN), IsUnique = true)]
    public class Car
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string VIN { get; set; }

        public virtual User Owner { get; set; }

        public virtual VehicleTrim Trim { get; set; }

        [Column(TypeName = "geography")]
        public required Point CurrentPosition { get; set; }

        public double RemainingReach { get; set; }

        public required string Colour { get; set; }

        public required int SeatNumbers { get; set; }

        public required double PricePerMinute { get; set; }

        public virtual CarStatus Status { get; private set; }

        public DateTime LastStatusChange { get; private set; } = DateTime.UtcNow;

        public TimeSpan RideTime => (CarStatuses)Status.Id != CarStatuses.InTransit
            ? TimeSpan.Zero
            : DateTime.UtcNow - LastStatusChange;

        public virtual Reservation? ActiveReservation { get; private set; }

        public Car()
        {

        }

        public Car(CarStatus initialStatus)
        {
            ArgumentNullException.ThrowIfNull(initialStatus);
            Status = initialStatus;
        }

        public Car SetStatus(CarStatus? newStatus, Reservation reservation)
        {
            ArgumentNullException.ThrowIfNull(newStatus, nameof(newStatus));

            var currentStatus = (CarStatuses)Status.Id;
            var nextStatus = (CarStatuses)newStatus.Id;

            switch (currentStatus)
            {
                case CarStatuses.Available:
                case CarStatuses.InTransit:
                    if (nextStatus != CarStatuses.Pending)
                        throw new ArgumentException($"A car can become Pending from {currentStatus}.");
                    break;

                case CarStatuses.Reserved:
                    if (nextStatus != CarStatuses.Available && nextStatus != CarStatuses.InTransit)
                        throw new ArgumentException("A car can only become Available again or become InTransit when it's Reserved.");

                    if (nextStatus != CarStatuses.InTransit && ActiveReservation?.Id != reservation.Id)
                        throw new ArgumentException("You must use the same reservation used to reserve the car to unlock it.");
                    break;

                case CarStatuses.Pending:
                    if (nextStatus != CarStatuses.Available && nextStatus != CarStatuses.Reserved)
                        throw new ArgumentException("A car can only become Available again or become Reserved when it's Pending.");

                    if (nextStatus != CarStatuses.Reserved && ActiveReservation?.Id != reservation.Id)
                        throw new ArgumentException("You must use the same reservation used to block the car to reserve it.");
                    break;
            }

            ActiveReservation = nextStatus != CarStatuses.Available ? reservation : null;
            Status = newStatus;
            LastStatusChange = DateTime.UtcNow;
            return this;
        }
    }
}
