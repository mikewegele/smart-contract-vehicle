using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartContractVehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public UserController(AppDbContext db, IMapper mapper, UserService userService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            _db.Users.Remove(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterTO dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully");
        }

        [HttpPatch]
        public IActionResult Update([FromBody] SmartContractVehicle.Model.User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.Users.Update(user);
            _db.SaveChanges();

            var updatedUser = _db.Users.Find(user.Id);
            if (updatedUser == null)
                return StatusCode(500, "Failed to update user");

            return Ok(updatedUser);
        }
<<<<<<< Updated upstream
=======

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserTO>(user);
            return Ok(userDto);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserTO userDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(userDto);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginTO loginDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var key = Encoding.ASCII.GetBytes("your_super_secret_key_here"); // Replace with your secure key
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return token and mapped user DTO for security
            var userDto = _mapper.Map<UserTO>(user);

            return Ok(new { token = tokenString, user = userDto });
        }
>>>>>>> Stashed changes
    }
}