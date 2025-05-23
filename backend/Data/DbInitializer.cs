using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace SmartContractVehicle.Data
{
    public static class DbInitializer
    {
        public const string RENTER = "renter";
        public const string LESSOR = "lessor";
        public const string ADMIN  = "admin";
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { RENTER, LESSOR, ADMIN };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }

}
