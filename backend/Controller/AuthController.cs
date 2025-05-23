using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Controller
{
    [Route("api/[controller]/[action]/")]
    public class AuthController(UserManager<User> uM, SignInManager<User> sM, IConfiguration conf) : ControllerBase
    {
        private readonly UserManager<User> _userManager = uM;
        private readonly SignInManager<User> _signInManager = sM;
        private readonly IConfiguration _config = conf;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterTO rto)
        {
            var user = new User { Name = rto.Name, Email = rto.Email };
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

            var token = generateJwtToken(user);
            return Ok(new { token });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("logged out.");
        }

    }
}
