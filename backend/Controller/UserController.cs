using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartContractVehicle.Data;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Service;
using AutoMapper;

namespace SmartContractVehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserController(AppDbContext db, IMapper mapper, UserService userService, IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _config = config;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterTO dto)
        {
            var createdUser = await _userService.CreateUserAsync(dto);
            return Ok(createdUser);
        }

        [HttpPatch]
        public IActionResult Update(SmartContractVehicle.Model.User user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _db.Users.Update(user);
            _db.SaveChanges();

            var updatedUser = _db.Users.Find(user.Id);
            if (updatedUser == null)
                return StatusCode(500, "User update failed");

            return Ok(updatedUser);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserTO>(user);
            return Ok(userDto);
        }

        // DTOs for login request and response
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public UserTO User { get; set; } = new UserTO();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var userDto = _mapper.Map<UserTO>(user);

            var token = GenerateJwtToken(user);

            var response = new LoginResponse
            {
                Token = token,
                User = userDto
            };

            return Ok(response);
        }

        private string GenerateJwtToken(SmartContractVehicle.Model.User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName),
                new Claim("isAdmin", user.IsAdmin.ToString()),
                new Claim("isLessor", user.IsLessor.ToString()),
                new Claim("isRenter", user.IsRenter.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}