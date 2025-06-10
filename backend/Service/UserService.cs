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
    public async Task<UserTO?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        return new UserTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin,
            IsLessor = user.IsLessor,
            IsRenter = user.IsRenter
        };
    }

    public async Task<UserTO?> UpdateUserAsync(UserTO dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id);
        if (user == null) return null;

        user.Name = dto.Name;
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.IsAdmin = dto.IsAdmin;
        user.IsRenter = dto.IsRenter;
        user.IsLessor = dto.IsLessor;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded) return null;

        return new UserTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin,
            IsLessor = user.IsLessor,
            IsRenter = user.IsRenter
        };
    }
    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid) return null;

        return user;
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangeUserPasswordAsync(User user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }
}
