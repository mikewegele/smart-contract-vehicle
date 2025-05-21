using User.Domain;
using User.Model;
using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Data;

namespace User.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User.Model.User> CreateUserAsync(NewUser newUser)
        {
            var user = new User.Model.User
            {
                Name = newUser.Name,
                Password = newUser.Password,
                Email = newUser.Email
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }
    }
}
