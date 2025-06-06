using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Attributes;

namespace SmartContractVehicle.Data;

[Flags]
public enum FuelTypes
{
    [Display("None")]
    None = 0,

    [Display("Gasoline")]
    Gasoline = 1 << 0,           // 1

    [Display("Diesel")]
    Diesel = 1 << 1,             // 2

    [Display("Flex Fuel")]
    FlexFuel = 1 << 2,           // 4

    [Display("Electric")]
    Electric = 1 << 3,           // 8

    [Display("Hybrid")]
    Hybrid = 1 << 4,             // 16

    [Display("Plug-in Hybrid")]
    PlugInHybrid = 1 << 5,       // 32

    [Display("Hydrogen")]
    Hydrogen = 1 << 6,           // 64

    [Display("Compressed Natural Gas (CNG)")]
    CompressedNaturalGas = 1 << 7,  // 128

    [Display("Liquefied Petroleum Gas (LPG)")]
    LiquefiedPetroleumGas = 1 << 8, // 256

    [Display("Biodiesel")]
    Biodiesel = 1 << 9,          // 512

    [Display("Ethanol")]
    Ethanol = 1 << 10            // 1024
}



public static class FuelTypeSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var fuelTypes = Enum.GetValues<FuelTypes>()
            .Cast<FuelTypes>()
            .Where(f => f != FuelTypes.None)  // skip 'None'
            .Select(f => new Model.FuelType
            {
                Id = (int)f,
                Name = GetDisplayName(f)
            });

        modelBuilder.Entity<Model.FuelType>().HasData(fuelTypes);
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
