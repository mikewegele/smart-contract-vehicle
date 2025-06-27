using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Attributes;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Data;
public enum CarStatuses
{
    [Display("Available")]
    Available = 1,

    [Display("Reserved")]
    Reserved = 2,

    [Display("In-Transit")]
    InTransit = 3,

    [Display("Pending")]
    Pending = 4
}

public static class CarStatusSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var carstatus = Enum.GetValues<CarStatuses>()
            .Select(d => new Model.CarStatus
            {
                Id = (int)d,
                Name = GetDisplayName(d)
            });

        modelBuilder.Entity<CarStatus>().HasData(carstatus);
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
