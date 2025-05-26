using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Service;


namespace SmartContractVehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]
    public class UserController(AppDbContext db, IMapper mapper, UserService userService) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly UserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Get(int id)
        {
            var user = _db.Users.Find(id);
            return Ok(user);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }

            return Ok();
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
            if (ModelState.IsValid)
            {
                _db.Users.Update(user);
                _db.SaveChanges();
                user = _db.Users.Find(user.Id);
                if (user != null)
                {
                    return StatusCode(500);
                }

                return Ok(user);
            }

                return BadRequest();
        }
    }
}
