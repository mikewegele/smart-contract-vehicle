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

        public virtual CarStatus Status { get; private set; } = new() { Id = (int)CarStatuses.Available, Name = "" };

        public DateTime LastStatusChange { get; private set; }

        public Car SetStatus(CarStatus? status)
        {
            ArgumentNullException.ThrowIfNull(status);

            LastStatusChange = DateTime.UtcNow;
            // TODO Logic for the different Status do we have to mind specific transits in Status?
            Status = status;
            return this;
        }

        public TimeSpan RideTime {
            get 
            {
                if ((CarStatuses)Status.Id != CarStatuses.InTransit)
                    return TimeSpan.Zero;
                else
                    return DateTime.UtcNow - LastStatusChange;
            }
        }
    }
}

