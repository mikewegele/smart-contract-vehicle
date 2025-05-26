using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartContractVehicle.Data;
using SmartContractVehicle.Models;


namespace SmartContractVehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]
    public class UserController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

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
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _db.Addresses.AddRange(user.Mailing, user.Billing);
                _db.Users.Add(user);
                _db.SaveChanges();
                return Ok(user);
            }
            else
            {
                return BadRequest();
            }            
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
    }
}
