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
            UserName = dto.Name,
            Email = dto.Email,
            Name = dto.Name,
            WalletId = dto.WalletId,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "renter"); // Standardrolle
        }

        return result;
    }
}
