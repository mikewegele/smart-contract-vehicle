using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Attributes;

namespace SmartContractVehicle.Data;
public enum Drivetrains
{
    [Display("Front-Wheel Drive (FWD)")]
    FrontWheelDrive = 1,

    [Display("Rear-Wheel Drive (RWD)")]
    RearWheelDrive = 2,

    [Display("All-Wheel Drive (AWD)")]
    AllWheelDrive = 3,

    [Display("Four-Wheel Drive (4WD)")]
    FourWheelDrive = 4
}

public static class DrivetrainSeeder
{    public static void Seed(ModelBuilder modelBuilder)
        {
            var drivetrains = Enum.GetValues<Data.Drivetrains>()
                .Select(d => new Model.Drivetrain
                {
                    Id = (int)d,
                    Name = GetDisplayName(d)
                });

            modelBuilder.Entity<Model.Drivetrain>().HasData(drivetrains);
        }

        private static string GetDisplayName(Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttributes(false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault()?.Name ?? value.ToString();
        }
}