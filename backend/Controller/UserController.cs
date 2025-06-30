using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Controller;

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
    public IActionResult Update(User user)
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

    [HttpGet("{id}")]
    public ActionResult<IQueryable<UserTO>> GetUserById(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        var userDto = _mapper.Map<UserTO>(user);
        return Ok(userDto);
    }

    [Authorize]
    [HttpGet]
    public ActionResult<UserTO> Profile()
    {
        var emailClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (emailClaim == null)
            return Unauthorized();

        var user = _db.Users.SingleOrDefault(u => u.Email == emailClaim);
        if (user == null)
            return NotFound();

        var userDto = _mapper.Map<UserTO>(user);
        return Ok(userDto);
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateTO dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(dto.Id);
        if (user == null || user.Id != dto.Id)
            return NotFound();

        _mapper.Map(dto, user);
        await _userService.UpdateUserAsync(user);

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                return BadRequest();

            if (dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest();

            var result = await _userService.ChangeUserPasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
        }

        return Ok(_mapper.Map<UserTO>(user));
    }

}
