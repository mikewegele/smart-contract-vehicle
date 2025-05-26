using Microsoft.AspNetCore.Identity;
using SmartContractVehicle.Model;
using SmartContractVehicle.DTO;

namespace SmartContractVehicle.Service;

public class UserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateUserAsync(RegisterTO dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            //WalletId = dto.WalletId,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return result;

        var rolesToAssign = new List<string>();

        if (user.IsAdmin)
            rolesToAssign.Add(Data.DbInitializer.ADMIN);

        if (user.IsLessor)
            rolesToAssign.Add(Data.DbInitializer.LESSOR);

        if (user.IsRenter)
            rolesToAssign.Add(Data.DbInitializer.RENTER);

        if (rolesToAssign.Any())
            await _userManager.AddToRolesAsync(user, rolesToAssign);

        return result;
    }
}
