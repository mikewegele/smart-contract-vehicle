using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.Data;

[Flags]
public enum FuelType
{
    [Display(Name = "None")]
    None = 0,

    [Display(Name = "Gasoline")]
    Gasoline = 1 << 0,           // 1

    [Display(Name = "Diesel")]
    Diesel = 1 << 1,             // 2

    [Display(Name = "Flex Fuel")]
    FlexFuel = 1 << 2,           // 4

    [Display(Name = "Electric")]
    Electric = 1 << 3,           // 8

    [Display(Name = "Hybrid")]
    Hybrid = 1 << 4,             // 16

    [Display(Name = "Plug-in Hybrid")]
    PlugInHybrid = 1 << 5,       // 32

    [Display(Name = "Hydrogen")]
    Hydrogen = 1 << 6,           // 64

    [Display(Name = "Compressed Natural Gas (CNG)")]
    CompressedNaturalGas = 1 << 7,  // 128

    [Display(Name = "Liquefied Petroleum Gas (LPG)")]
    LiquefiedPetroleumGas = 1 << 8, // 256

    [Display(Name = "Biodiesel")]
    Biodiesel = 1 << 9,          // 512

    [Display(Name = "Ethanol")]
    Ethanol = 1 << 10            // 1024
}



public static class FuelTypeSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var fuelTypes = Enum.GetValues<FuelType>()
            .Cast<FuelType>()
            .Where(f => f != FuelType.None)  // skip 'None'
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
