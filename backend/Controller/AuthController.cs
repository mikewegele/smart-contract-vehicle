using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]/")]
    public class AuthController(UserManager<User> uM, SignInManager<User> sM, IConfiguration conf) : ControllerBase
    {
        private readonly UserManager<User> _userManager = uM;
        private readonly SignInManager<User> _signInManager = sM;
        private readonly IConfiguration _config = conf;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterTO rto)
        {
            Console.WriteLine($"EMAIL: {rto.Email}, PASSWORD: {rto.Password}, NAME: {rto.Name}");

            if (string.IsNullOrEmpty(rto.Email) || string.IsNullOrEmpty(rto.Password) || string.IsNullOrEmpty(rto.Name))
                return BadRequest("Missing data");

            var user = new User { UserName = rto.Email, Name = rto.Name, Email = rto.Email };
            var res = await _userManager.CreateAsync(user, rto.Password);
            if (res.Succeeded)
                return Ok("Registered.");

            return BadRequest(res.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginTO lto)
        {
            var user = await _userManager.FindByEmailAsync(lto.Email);
            if (user is null)
                return Unauthorized("Invalid email.");

            var res = await _signInManager.CheckPasswordSignInAsync(user, lto.Password, false);
            if (!res.Succeeded)
                return Unauthorized("Invalid password.");

            var token = await GenerateJwtTokenAsync(user);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("logged out.");
        }


        
        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Name, user.Name)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var expDate = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: expDate,
                signingCredentials : cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
